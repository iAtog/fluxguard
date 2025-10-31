using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace fluxguard
{
    public partial class Main : Form
    {
        private Process backendProcess;
        private Process embeddingsProcess;

        private volatile bool backendStartedDetected = false;
        private volatile bool embeddingsStartedDetected = false;
        private bool autoStartSequenceStarted = false;
        private const string AppRegKey = "Software\\Fluxguard";
        private bool reallyExit = false;

        private string GetSavedProjectPath()
        {
            try
            {
                using (var rk = Registry.CurrentUser.OpenSubKey(AppRegKey, false))
                {
                    if (rk == null) return null;
                    var val = rk.GetValue("ProjectPath") as string;
                    return string.IsNullOrWhiteSpace(val) ? null : val;
                }
            }
            catch { return null; }
        }

        private bool SaveProjectPath(string path)
        {
            try
            {
                using (var rk = Registry.CurrentUser.CreateSubKey(AppRegKey))
                {
                    if (rk == null) return false;
                    if (string.IsNullOrWhiteSpace(path)) rk.DeleteValue("ProjectPath", false);
                    else rk.SetValue("ProjectPath", path, RegistryValueKind.String);
                }
                return true;
            }
            catch { return false; }
        }

        private void projectUrlTxt_TextChanged(object sender, EventArgs e)
        {
            try { SaveProjectPath(projectUrlTxt.Text.Trim()); } catch { }
        }

        private bool IsStartMinimizedEnabled()
        {
            try
            {
                using (var rk = Registry.CurrentUser.OpenSubKey(AppRegKey, false))
                {
                    if (rk == null) return false;
                    var val = rk.GetValue("StartMinimized");
                    if (val == null) return false;
                    int ival;
                    if (int.TryParse(val.ToString(), out ival))
                        return ival != 0;
                    bool bval;
                    if (bool.TryParse(val.ToString(), out bval))
                        return bval;
                    return false;
                }
            }
            catch { return false; }
        }

        private bool SetStartMinimized(bool enable)
        {
            try
            {
                using (var rk = Registry.CurrentUser.CreateSubKey(AppRegKey))
                {
                    if (rk == null) return false;
                    rk.SetValue("StartMinimized", enable ? 1 : 0, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public Main()
        {
            InitializeComponent();
            this.Resize += hideInTray;

            notifyIcon1.Visible = false;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.BalloonTipTitle = "Fluxguard";
            notifyIcon1.BalloonTipText = "La aplicación se ha minimizado en segundo plano.";
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;

            checkTimer.Enabled = true;
        }

        private void hideInTray(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                this.Show();
                notifyIcon1.Visible = false;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            UpdateEstadoLabels(false, false);
            try
            {
                startWithWindowsChk.Checked = IsStartupEnabled();
            }
            catch { }

            try
            {
                var saved = GetSavedProjectPath();
                if (!string.IsNullOrEmpty(saved)) projectUrlTxt.Text = saved;
                projectUrlTxt.TextChanged -= projectUrlTxt_TextChanged;
                projectUrlTxt.TextChanged += projectUrlTxt_TextChanged;
            }
            catch { }

            try
            {
                StartEmbeddings();
            }
            catch { }

            try
            {
                bool startMin = IsStartMinimizedEnabled();
                startMinimizedBtn.Text = startMin ? "Iniciar minimizado: Sí" : "Iniciar minimizado: No";
            }
            catch { }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try
            {
                if (IsStartMinimizedEnabled())
                {
                    this.WindowState = FormWindowState.Minimized;
                    notifyIcon1.Visible = true;
                    notifyIcon1.ShowBalloonTip(500);
                    this.Hide();
                }
            }
            catch { }
        }

        private bool IsStartupEnabled()
        {
            try
            {
                using (var rk = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", false))
                {
                    if (rk == null) return false;
                    var val = rk.GetValue("Fluxguard") as string;
                    if (string.IsNullOrEmpty(val)) return false;
                    return val.IndexOf(Application.ExecutablePath, StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool SetStartup(bool enable)
        {
            try
            {
                using (var rk = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (rk == null) return false;
                    if (enable)
                    {
                        rk.SetValue("Fluxguard", "\"" + Application.ExecutablePath + "\"");
                    }
                    else
                    {
                        rk.DeleteValue("Fluxguard", false);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void startWithWindowsChk_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = startWithWindowsChk.Checked;
            bool ok = SetStartup(enable);
            if (!ok)
            {
                MessageBox.Show("No se pudo modificar la clave de inicio. Ejecuta la aplicación con permisos adecuados.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { startWithWindowsChk.CheckedChanged -= startWithWindowsChk_CheckedChanged; startWithWindowsChk.Checked = !enable; startWithWindowsChk.CheckedChanged += startWithWindowsChk_CheckedChanged; } catch { }
            }
        }

        private void startMinimizedBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool current = IsStartMinimizedEnabled();
                bool newVal = !current;
                bool ok = SetStartMinimized(newVal);
                if (!ok)
                {
                    MessageBox.Show("No se pudo actualizar la configuración. Comprueba permisos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                startMinimizedBtn.Text = newVal ? "Iniciar minimizado: Sí" : "Iniciar minimizado: No";
                if (newVal)
                {
                    this.WindowState = FormWindowState.Minimized;
                    notifyIcon1.Visible = true;
                    notifyIcon1.ShowBalloonTip(500);
                    this.Hide();
                }
                else
                {
                    if (!this.Visible)
                    {
                        this.Show();
                    }
                    this.WindowState = FormWindowState.Normal;
                    notifyIcon1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized || !this.Visible)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
                this.Activate();
                notifyIcon1.Visible = false;
            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reallyExit = true;
             this.Close();
        }

        private async void checkTimer_Tick(object sender, EventArgs e)
        {
            bool backendOn = backendStartedDetected;
            bool embeddingsOn = embeddingsStartedDetected;

            try
            {
                if (backendProcess != null)
                {
                    bool backendExited = false;
                    try { backendExited = backendProcess.HasExited; }
                    catch (InvalidOperationException)
                    {
                        backendExited = true;
                    }
                    if (backendExited)
                    {
                        try { backendProcess.Dispose(); } catch { }
                        backendProcess = null;
                        backendStartedDetected = false;
                        AppendBackendLog("[INFO] Se finalizó el proceso del servidor");
                    }
                }
            }
            catch { }

            try
            {
                if (embeddingsProcess != null)
                {
                    bool embExited = false;
                    try { embExited = embeddingsProcess.HasExited; }
                    catch (InvalidOperationException)
                    {
                        embExited = true;
                    }
                    if (embExited)
                    {
                        try { embeddingsProcess.Dispose(); } catch { }
                        embeddingsProcess = null;
                        embeddingsStartedDetected = false;
                        AppendEmbeddingsLog("[INFO] Se finalizó el proceso de embeddings");
                    }
                }
            }
            catch { }

            UpdateEstadoLabels(backendOn, embeddingsOn);
        }

        private void AppendBackendLog(string text)
        {
            if (backendLogBox.InvokeRequired)
            {
                backendLogBox.BeginInvoke(new Action(() => AppendBackendLog(text)));
                return;
            }
            backendLogBox.AppendText(text + Environment.NewLine);
            backendLogBox.ScrollToCaret();
        }

        private void AppendEmbeddingsLog(string text)
        {
            if (embeddingsLogBox.InvokeRequired)
            {
                embeddingsLogBox.BeginInvoke(new Action(() => AppendEmbeddingsLog(text)));
                return;
            }
            embeddingsLogBox.AppendText(text + Environment.NewLine);
            embeddingsLogBox.ScrollToCaret();
        }

        private void UpdateEstadoLabels(bool backendOn, bool embeddingsOn)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateEstadoLabels(backendOn, embeddingsOn)));
                return;
            }

            backendEstado.Text = state(backendOn, backendProcess);
            backendEstado.ForeColor = color(backendOn, backendProcess);

            embeddingsEstado.Text = state(embeddingsOn, embeddingsProcess);
            embeddingsEstado.ForeColor = color(embeddingsOn, embeddingsProcess);
        }

        private string state(bool srv, object process)
        {
            return srv ? "Encendido" : (process != null ? "Iniciando..." : "Apagado");
        }

        private Color color(bool srv, object process)
        {
            return srv ? Color.Green : (process != null ? Color.Orange : Color.Red);
        }

        private void backendToggle_Click(object sender, EventArgs e)
        {
            if (backendProcess != null && !backendProcess.HasExited)
            {
                StopProcess(ref backendProcess, ref backendStartedDetected, AppendBackendLog, "backend");
                UpdateEstadoLabels(false, embeddingsStartedDetected);
                return;
            }

            StartBackend();
        }

        private void embeddingsToggle_Click(object sender, EventArgs e)
        {
            if (embeddingsProcess != null && !embeddingsProcess.HasExited)
            {
                StopProcess(ref embeddingsProcess, ref embeddingsStartedDetected, AppendEmbeddingsLog, "embeddings");
                UpdateEstadoLabels(backendStartedDetected, false);
                return;
            }

            StartEmbeddings();
        }

        private void StartEmbeddings()
        {
            if (embeddingsProcess != null && !embeddingsProcess.HasExited) return;

            string folder = projectUrlTxt.Text.Trim();
            if (string.IsNullOrEmpty(folder))
            {
                MessageBox.Show("No se puede dejar vacío la dirección del proyecto en disco", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppendEmbeddingsLog("[INFO] Iniciando embeddings en: " + folder);
            embeddingsStartedDetected = false;
            UpdateEstadoLabels(backendStartedDetected, false);

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c npm run embeddings",
                    WorkingDirectory = folder,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                embeddingsProcess = new Process { StartInfo = psi, EnableRaisingEvents = true };
                embeddingsProcess.OutputDataReceived += (s, ev) =>
                {
                    if (ev.Data == null) return;
                    AppendEmbeddingsLog(ev.Data);
                    if (!embeddingsStartedDetected && ev.Data.IndexOf("ready, listening", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        embeddingsStartedDetected = true;
                        UpdateEstadoLabels(backendStartedDetected, true);
                        if (!autoStartSequenceStarted)
                        {
                            autoStartSequenceStarted = true;
                            StartBackend();
                        }
                    }
                };
                embeddingsProcess.ErrorDataReceived += (s, ev) =>
                {
                    if (ev.Data == null) return;
                    AppendEmbeddingsLog("[ERR] " + ev.Data);
                    if (!embeddingsStartedDetected && ev.Data.IndexOf("ready, listening", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        embeddingsStartedDetected = true;
                        UpdateEstadoLabels(backendStartedDetected, true);
                        if (!autoStartSequenceStarted)
                        {
                            autoStartSequenceStarted = true;
                            StartBackend();
                        }
                    }
                };
                embeddingsProcess.Exited += (s, ev) =>
                {
                    embeddingsStartedDetected = false;
                    AppendEmbeddingsLog("[INFO] Embeddings finalizado");
                };

                embeddingsProcess.Start();
                embeddingsProcess.BeginOutputReadLine();
                embeddingsProcess.BeginErrorReadLine();

                UpdateEstadoLabels(backendStartedDetected, false);
            }
            catch (Exception ex)
            {
                AppendEmbeddingsLog("[Error] No se pudo iniciar embeddings: " + ex.Message);
                UpdateEstadoLabels(backendStartedDetected, false);
            }
        }

        private void StartBackend()
        {
            if (backendProcess != null && !backendProcess.HasExited) return;

            string folder = projectUrlTxt.Text.Trim();
            if (string.IsNullOrEmpty(folder))
            {
                AppendBackendLog("[ERROR] No se inició backend porque la ruta del proyecto está vacía");
                return;
            }

            AppendBackendLog("[INFO] Iniciando servidor en: " + folder);
            backendStartedDetected = false;
            UpdateEstadoLabels(false, embeddingsStartedDetected);

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c npm start",
                    WorkingDirectory = folder,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                backendProcess = new Process { StartInfo = psi, EnableRaisingEvents = true };
                backendProcess.OutputDataReceived += (s, ev) =>
                {
                    if (ev.Data == null) return;
                    AppendBackendLog(ev.Data);
                    if (!backendStartedDetected && ev.Data.IndexOf("Servidor iniciado en", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        backendStartedDetected = true;
                        UpdateEstadoLabels(true, embeddingsStartedDetected);
                    }
                };
                backendProcess.ErrorDataReceived += (s, ev) =>
                {
                    if (ev.Data == null) return;
                    AppendBackendLog("[err] " + ev.Data);
                    if (!backendStartedDetected && ev.Data.IndexOf("Servidor iniciado en", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        backendStartedDetected = true;
                        UpdateEstadoLabels(true, embeddingsStartedDetected);
                    }
                };
                backendProcess.Exited += (s, ev) =>
                {
                    backendStartedDetected = false;
                    AppendBackendLog("[INFO] Servidor finalizado");
                };

                backendProcess.Start();
                backendProcess.BeginOutputReadLine();
                backendProcess.BeginErrorReadLine();

                UpdateEstadoLabels(false, embeddingsStartedDetected);
            }
            catch (Exception ex)
            {
                AppendBackendLog("[ERROR] No se pudo iniciar backend: " + ex.Message);
                UpdateEstadoLabels(false, embeddingsStartedDetected);
            }
        }

        private void KillProcessTreeByPid(int pid)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = $"/PID {pid} /T /F",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                using (var p = Process.Start(psi))
                {
                    p.WaitForExit(3000);
                }
            }
            catch { }
        }

        private void StopProcess(ref Process proc, ref bool startedDetected, Action<string> appendLog, string name)
        {
            if (proc == null) return;
            try
            {
                try
                {
                    appendLog($"[INFO] deteniendo {name} (pid={proc.Id})...");
                }
                catch { appendLog($"[INFO] deteniendo {name} (pid=N/A)..."); }

                try { KillProcessTreeByPid(proc.Id); } catch { }

                try
                {
                    if (!proc.WaitForExit(5000))
                    {
                        try { proc.Kill(); } catch { }
                    }
                }
                catch {  }
            }
            catch (Exception ex)
            {
                try { appendLog("[ERROR] Error deteniendo " + name + ": " + ex.Message); } catch { }
            }
            finally
            {
                try { proc.Dispose(); } catch { }
                proc = null;
                startedDetected = false;
                try { appendLog("[INFO] " + name + " detenido"); } catch { }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!reallyExit)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                notifyIcon1.Visible = true;
                this.Hide();
                return;
            }

            checkTimer.Enabled = false;
            StopProcess(ref backendProcess, ref backendStartedDetected, AppendBackendLog, "backend");
            StopProcess(ref embeddingsProcess, ref embeddingsStartedDetected, AppendEmbeddingsLog, "embeddings");
            base.OnFormClosing(e);
         }
     }
 }