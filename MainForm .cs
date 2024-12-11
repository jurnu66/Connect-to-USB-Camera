using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace USB_Camera_Recorder
{
    public partial class MainForm : Form
    {
        private VideoCapture _camera;
        private bool _isRecording = false;

        public MainForm()
        {
            InitializeComponent();
            UpdateConnectionStatus("Disconnected");
            UpdateRecordStatus("Pause");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_camera == null)
            {
                try
                {
                    _camera = new VideoCapture(0); // Camera index 0
                    _camera.ImageGrabbed += ProcessFrame;
                    _camera.Start();

                    UpdateConnectionStatus("Connected");
                    btnConnect.Text = "Disconnect";
                }
                catch (Exception ex)
                {
                    UpdateConnectionStatus("Disconnected");
                    MessageBox.Show($"Error connecting to camera: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                _camera.Dispose();
                _camera = null;
                UpdateConnectionStatus("Disconnected");
                btnConnect.Text = "Connect";
            }
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (_camera == null)
            {
                MessageBox.Show("Please connect the camera first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isRecording = !_isRecording;
            UpdateRecordStatus(_isRecording ? "Play" : "Pause");
            btnRecord.Text = _isRecording ? "Pause" : "Play";
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            try
            {
                if (_camera != null && _camera.Ptr != IntPtr.Zero)
                {
                    var frame = new Mat();
                    _camera.Retrieve(frame);
                    pictureBoxCamera.Image = frame.ToImage<Bgr, byte>().ToBitmap();
                }
            }
            catch (Exception ex)
            {
                UpdateConnectionStatus("Disconnected");
                MessageBox.Show($"Error processing frame: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateConnectionStatus(string status)
        {
            lblConnectionStatus.Text = $"Camera Status: {status}";
            lblConnectionStatus.ForeColor = status == "Connected" ? Color.Green : Color.Red;
        }

        private void UpdateRecordStatus(string status)
        {
            lblRecordStatus.Text = $"Recording: {status}";
            lblRecordStatus.ForeColor = status == "Play" ? Color.Green : Color.Red;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_camera != null)
            {
                _camera.Dispose();
            }
        }
    }
}
