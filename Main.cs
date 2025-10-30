using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fluxguard
{
    public partial class Main : Form
    {
        private Process backendProcess;
        private Process embeddingsProcess;

        private volatile bool backendStartedDetected = false;
        private volatile bool embeddingsStartedDetected = false;

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
            this.Close();
        }

        private async void checkTimer_Tick(object sender, EventArgs e)
        {
            bool backendOn = backendStartedDetected;
            bool embeddingsOn = embeddingsStartedDetected;

            if (backendProcess != null && backendProcess.HasExited)
            {
                backendProcess.Dispose();
                backendProcess = null;
                backendStartedDetected = false;
                AppendBackendLog("[INFO] Se finalizó el proceso del servidor");
            }
            if (embeddingsProcess != null && embeddingsProcess.HasExited)
            {
                embeddingsProcess.Dispose();
                embeddingsProcess = null;
                embeddingsStartedDetected = false;
                AppendEmbeddingsLog("[INFO] Se finalizó el proceso de embeddings");
            }

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
                try
                {
                    AppendBackendLog("[INFO] Deteniendo servidor...");
                    KillProcessTreeByPid(backendProcess.Id);
                    backendProcess.WaitForExit(2000);
                }
                catch (Exception ex)
                {
                    AppendBackendLog("[INFO] Error deteniendo servidor: " + ex.Message);
                }
                finally
                {
                    try { backendProcess.Kill(); } catch { }
                    try { backendProcess.Dispose(); } catch { }
                    backendProcess = null;
                    backendStartedDetected = false;
                    UpdateEstadoLabels(false, embeddingsStartedDetected);
                }
                return;
            }

            string folder = projectUrlTxt.Text.Trim();
            if (string.IsNullOrEmpty(folder))
            {
                MessageBox.Show("No se puede dejar vacío la dirección del proyecto en disco", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (!backendStartedDetected && ev.Data.IndexOf("Server listening at", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        backendStartedDetected = true;
                        UpdateEstadoLabels(true, embeddingsStartedDetected);
                    }
                };
                backendProcess.ErrorDataReceived += (s, ev) =>
                {
                    if (ev.Data == null) return;
                    AppendBackendLog("[err] " + ev.Data);
                    if (!backendStartedDetected && ev.Data.IndexOf("Server listening at", StringComparison.OrdinalIgnoreCase) >= 0)
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

        private void embeddingsToggle_Click(object sender, EventArgs e)
        {
            if (embeddingsProcess != null && !embeddingsProcess.HasExited)
            {
                try
                {
                    AppendEmbeddingsLog("[INFO] Deteniendo embeddings...");
                    KillProcessTreeByPid(embeddingsProcess.Id);
                    embeddingsProcess.WaitForExit(2000);
                }
                catch (Exception ex)
                {
                    AppendEmbeddingsLog("[ERROR] Error deteniendo embeddings: " + ex.Message);
                }
                finally
                {
                    try { embeddingsProcess.Kill(); } catch { }
                    try { embeddingsProcess.Dispose(); } catch { }
                    embeddingsProcess = null;
                    embeddingsStartedDetected = false;
                    UpdateEstadoLabels(backendStartedDetected, false);
                }
                return;
            }

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
                appendLog($"[INFO] deteniendo {name} (pid={proc.Id})...");
                KillProcessTreeByPid(proc.Id);
                if (!proc.WaitForExit(5000))
                {
                    try { proc.Kill(); } catch { }
                }
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
            checkTimer.Enabled = false;
            StopProcess(ref backendProcess, ref backendStartedDetected, AppendBackendLog, "backend");
            StopProcess(ref embeddingsProcess, ref embeddingsStartedDetected, AppendEmbeddingsLog, "embeddings");
            base.OnFormClosing(e);
        }
    }
}