#nullable enable

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace AppRSA
{
    public class Form1 : Form
    {
        // =========================================================
        // RSA KEYS
        // =========================================================

        private RSA? rsa;
        private RSA? publicKey;
        private RSA? privateKey;

        private byte[]? originalData;

        // =========================================================
        // CONTROLS
        // =========================================================

        private Button btnGenerateKey = null!;
        private Button btnExportPublic = null!;
        private Button btnExportPrivate = null!;
        private Button btnImportPublic = null!;
        private Button btnImportPrivate = null!;

        private TextBox txtParameters = null!;

        private TextBox txtInput = null!;
        private TextBox txtEncrypted = null!;
        private TextBox txtDecrypted = null!;

        private Button btnEncrypt = null!;
        private Button btnDecrypt = null!;
        private Button btnSaveEncrypted = null!;
        private Button btnLoadEncrypted = null!;

        private TextBox txtSpeed = null!;
        private Button btnSpeed = null!;

        private TextBox txtHashInput = null!;
        private TextBox txtHash = null!;
        private Button btnHash = null!;

        private TextBox txtHashInput1 = null!;
        private TextBox txtHashInput2 = null!;
        private Button btnAvalanche = null!;

        private TextBox txtSignData = null!;
        private TextBox txtSignature = null!;
        private TextBox txtSignatureCompare = null!;

        private Button btnSign = null!;
        private Button btnCompareSignature = null!;
        private Button btnVerify = null!;

        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public Form1()
        {
            InitializeComponent();
        }

        // =========================================================
        // TẠO GIAO DIỆN
        // =========================================================

        private void InitializeComponent()
        {
            this.Text = "Ứng dụng RSA - AES/RSA Assignment";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1100, 750);
            this.MinimumSize = new Size(900, 650);

            TabControl tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;

            // =====================================================
            // TAB 1 - KEY
            // =====================================================

            TabPage tabKey = new TabPage("1. Khóa RSA");

            btnGenerateKey = new Button();
            btnGenerateKey.Text = "Sinh cặp khóa RSA-2048";
            btnGenerateKey.SetBounds(20, 20, 200, 40);
            btnGenerateKey.Click += btnGenerateKey_Click;

            btnExportPublic = new Button();
            btnExportPublic.Text = "Xuất Public PEM";
            btnExportPublic.SetBounds(240, 20, 160, 40);
            btnExportPublic.Click += btnExportPublic_Click;

            btnExportPrivate = new Button();
            btnExportPrivate.Text = "Xuất Private PEM";
            btnExportPrivate.SetBounds(410, 20, 160, 40);
            btnExportPrivate.Click += btnExportPrivate_Click;

            btnImportPublic = new Button();
            btnImportPublic.Text = "Import Public PEM";
            btnImportPublic.SetBounds(580, 20, 160, 40);
            btnImportPublic.Click += btnImportPublic_Click;

            btnImportPrivate = new Button();
            btnImportPrivate.Text = "Import Private PEM";
            btnImportPrivate.SetBounds(750, 20, 160, 40);
            btnImportPrivate.Click += btnImportPrivate_Click;

            txtParameters = new TextBox();
            txtParameters.Multiline = true;
            txtParameters.ScrollBars = ScrollBars.Both;
            txtParameters.Font = new Font("Consolas", 10);
            txtParameters.SetBounds(20, 80, 1020, 570);
            txtParameters.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            tabKey.Controls.Add(btnGenerateKey);
            tabKey.Controls.Add(btnExportPublic);
            tabKey.Controls.Add(btnExportPrivate);
            tabKey.Controls.Add(btnImportPublic);
            tabKey.Controls.Add(btnImportPrivate);
            tabKey.Controls.Add(txtParameters);

            // =====================================================
            // TAB 2 - RSA ENCRYPT / DECRYPT
            // =====================================================

            TabPage tabRSA = new TabPage("2. Mã hóa / Giải mã");

            Label lblInput = new Label();
            lblInput.Text = "Dữ liệu đầu vào (≤ 190 byte):";
            lblInput.SetBounds(20, 20, 250, 25);

            txtInput = new TextBox();
            txtInput.Multiline = true;
            txtInput.ScrollBars = ScrollBars.Vertical;
            txtInput.SetBounds(20, 50, 1020, 80);
            txtInput.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            btnEncrypt = new Button();
            btnEncrypt.Text = "MÃ HÓA bằng PUBLIC KEY";
            btnEncrypt.SetBounds(20, 145, 220, 40);
            btnEncrypt.Click += btnEncrypt_Click;

            btnSaveEncrypted = new Button();
            btnSaveEncrypted.Text = "Lưu Base64";
            btnSaveEncrypted.SetBounds(250, 145, 150, 40);
            btnSaveEncrypted.Click += btnSaveEncrypted_Click;

            btnLoadEncrypted = new Button();
            btnLoadEncrypted.Text = "Đọc Base64";
            btnLoadEncrypted.SetBounds(410, 145, 150, 40);
            btnLoadEncrypted.Click += btnLoadEncrypted_Click;

            Label lblEncrypted = new Label();
            lblEncrypted.Text = "Dữ liệu mã hóa - Base64:";
            lblEncrypted.SetBounds(20, 200, 250, 25);

            txtEncrypted = new TextBox();
            txtEncrypted.Multiline = true;
            txtEncrypted.ScrollBars = ScrollBars.Both;
            txtEncrypted.Font = new Font("Consolas", 9);
            txtEncrypted.SetBounds(20, 230, 1020, 120);
            txtEncrypted.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            btnDecrypt = new Button();
            btnDecrypt.Text = "GIẢI MÃ bằng PRIVATE KEY";
            btnDecrypt.SetBounds(20, 365, 220, 40);
            btnDecrypt.Click += btnDecrypt_Click;

            Label lblDecrypted = new Label();
            lblDecrypted.Text = "Kết quả giải mã:";
            lblDecrypted.SetBounds(20, 420, 250, 25);

            txtDecrypted = new TextBox();
            txtDecrypted.Multiline = true;
            txtDecrypted.SetBounds(20, 450, 1020, 100);
            txtDecrypted.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            tabRSA.Controls.Add(lblInput);
            tabRSA.Controls.Add(txtInput);
            tabRSA.Controls.Add(btnEncrypt);
            tabRSA.Controls.Add(btnSaveEncrypted);
            tabRSA.Controls.Add(btnLoadEncrypted);
            tabRSA.Controls.Add(lblEncrypted);
            tabRSA.Controls.Add(txtEncrypted);
            tabRSA.Controls.Add(btnDecrypt);
            tabRSA.Controls.Add(lblDecrypted);
            tabRSA.Controls.Add(txtDecrypted);

            // =====================================================
            // TAB 3 - PERFORMANCE
            // =====================================================

            TabPage tabSpeed = new TabPage("3. Đánh giá tốc độ");

            btnSpeed = new Button();
            btnSpeed.Text = "Chạy kiểm tra 1000 lần";
            btnSpeed.SetBounds(20, 20, 220, 40);
            btnSpeed.Click += btnSpeed_Click;

            txtSpeed = new TextBox();
            txtSpeed.Multiline = true;
            txtSpeed.Font = new Font("Consolas", 12);
            txtSpeed.SetBounds(20, 80, 1020, 500);
            txtSpeed.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            tabSpeed.Controls.Add(btnSpeed);
            tabSpeed.Controls.Add(txtSpeed);

            // =====================================================
            // TAB 4 - SHA-256
            // =====================================================

            TabPage tabHash = new TabPage("4. SHA-256");

            Label lblHashInput = new Label();
            lblHashInput.Text = "Dữ liệu:";
            lblHashInput.SetBounds(20, 20, 100, 25);

            txtHashInput = new TextBox();
            txtHashInput.SetBounds(20, 50, 800, 35);
            txtHashInput.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            btnHash = new Button();
            btnHash.Text = "Sinh SHA-256";
            btnHash.SetBounds(840, 48, 180, 40);
            btnHash.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;
            btnHash.Click += btnHash_Click;

            txtHash = new TextBox();
            txtHash.Multiline = true;
            txtHash.ScrollBars = ScrollBars.Both;
            txtHash.Font = new Font("Consolas", 10);
            txtHash.SetBounds(20, 100, 1000, 180);
            txtHash.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            Label lblA1 = new Label();
            lblA1.Text = "Dữ liệu 1:";
            lblA1.SetBounds(20, 310, 100, 25);

            txtHashInput1 = new TextBox();
            txtHashInput1.SetBounds(20, 340, 450, 35);

            Label lblA2 = new Label();
            lblA2.Text = "Dữ liệu 2:";
            lblA2.SetBounds(500, 310, 100, 25);

            txtHashInput2 = new TextBox();
            txtHashInput2.SetBounds(500, 340, 450, 35);

            btnAvalanche = new Button();
            btnAvalanche.Text = "Đánh giá Avalanche Effect";
            btnAvalanche.SetBounds(20, 390, 250, 40);
            btnAvalanche.Click += btnAvalanche_Click;

            tabHash.Controls.Add(lblHashInput);
            tabHash.Controls.Add(txtHashInput);
            tabHash.Controls.Add(btnHash);
            tabHash.Controls.Add(txtHash);
            tabHash.Controls.Add(lblA1);
            tabHash.Controls.Add(txtHashInput1);
            tabHash.Controls.Add(lblA2);
            tabHash.Controls.Add(txtHashInput2);
            tabHash.Controls.Add(btnAvalanche);

            // =====================================================
            // TAB 5 - DIGITAL SIGNATURE
            // =====================================================

            TabPage tabSignature = new TabPage("5. Chữ ký số");

            Label lblSign = new Label();
            lblSign.Text = "Dữ liệu cần ký:";
            lblSign.SetBounds(20, 20, 150, 25);

            txtSignData = new TextBox();
            txtSignData.Multiline = true;
            txtSignData.SetBounds(20, 50, 1000, 80);
            txtSignData.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            btnSign = new Button();
            btnSign.Text = "KÝ SỐ";
            btnSign.SetBounds(20, 145, 150, 40);
            btnSign.Click += btnSign_Click;

            btnCompareSignature = new Button();
            btnCompareSignature.Text = "Ký 2 lần để so sánh";
            btnCompareSignature.SetBounds(180, 145, 180, 40);
            btnCompareSignature.Click += btnCompareSignature_Click;

            btnVerify = new Button();
            btnVerify.Text = "Xác minh chữ ký";
            btnVerify.SetBounds(370, 145, 170, 40);
            btnVerify.Click += btnVerify_Click;

            Label lblSignature = new Label();
            lblSignature.Text = "Chữ ký Base64:";
            lblSignature.SetBounds(20, 200, 150, 25);

            txtSignature = new TextBox();
            txtSignature.Multiline = true;
            txtSignature.ScrollBars = ScrollBars.Both;
            txtSignature.Font = new Font("Consolas", 9);
            txtSignature.SetBounds(20, 230, 1000, 130);
            txtSignature.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            Label lblCompare = new Label();
            lblCompare.Text = "So sánh 2 chữ ký:";
            lblCompare.SetBounds(20, 380, 200, 25);

            txtSignatureCompare = new TextBox();
            txtSignatureCompare.Multiline = true;
            txtSignatureCompare.ScrollBars = ScrollBars.Both;
            txtSignatureCompare.Font = new Font("Consolas", 9);
            txtSignatureCompare.SetBounds(20, 410, 1000, 180);
            txtSignatureCompare.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            tabSignature.Controls.Add(lblSign);
            tabSignature.Controls.Add(txtSignData);
            tabSignature.Controls.Add(btnSign);
            tabSignature.Controls.Add(btnCompareSignature);
            tabSignature.Controls.Add(btnVerify);
            tabSignature.Controls.Add(lblSignature);
            tabSignature.Controls.Add(txtSignature);
            tabSignature.Controls.Add(lblCompare);
            tabSignature.Controls.Add(txtSignatureCompare);

            // =====================================================
            // ADD TABS
            // =====================================================

            tabs.TabPages.Add(tabKey);
            tabs.TabPages.Add(tabRSA);
            tabs.TabPages.Add(tabSpeed);
            tabs.TabPages.Add(tabHash);
            tabs.TabPages.Add(tabSignature);

            this.Controls.Add(tabs);
        }

        // =========================================================
        // SINH KHÓA
        // =========================================================

        private void GenerateRSAKey()
        {
            rsa?.Dispose();
            publicKey?.Dispose();
            privateKey?.Dispose();

            rsa = RSA.Create(2048);

            publicKey = RSA.Create();
            publicKey.ImportParameters(
                rsa.ExportParameters(false)
            );

            privateKey = RSA.Create();
            privateKey.ImportParameters(
                rsa.ExportParameters(true)
            );

            MessageBox.Show(
                "Đã sinh cặp khóa RSA-2048!"
            );
        }

        // =========================================================
        // EXPORT PUBLIC KEY
        // =========================================================

        private void ExportPublicKeyPem(string filePath)
        {
            if (rsa == null)
                throw new Exception("Chưa sinh khóa!");

            string pem =
                rsa.ExportSubjectPublicKeyInfoPem();

            File.WriteAllText(
                filePath,
                pem,
                Encoding.ASCII
            );
        }

        // =========================================================
        // EXPORT PRIVATE KEY
        // =========================================================

        private void ExportPrivateKeyPem(string filePath)
        {
            if (rsa == null)
                throw new Exception("Chưa sinh khóa!");

            string pem =
                rsa.ExportPkcs8PrivateKeyPem();

            File.WriteAllText(
                filePath,
                pem,
                Encoding.ASCII
            );
        }

        // =========================================================
        // IMPORT PUBLIC
        // =========================================================

        private RSA ImportPublicKey(string filePath)
        {
            string pem =
                File.ReadAllText(
                    filePath,
                    Encoding.ASCII
                );

            RSA key = RSA.Create();

            key.ImportFromPem(pem);

            return key;
        }

        // =========================================================
        // IMPORT PRIVATE
        // =========================================================

        private RSA ImportPrivateKey(string filePath)
        {
            string pem =
                File.ReadAllText(
                    filePath,
                    Encoding.ASCII
                );

            RSA key = RSA.Create();

            key.ImportFromPem(pem);

            return key;
        }

        // =========================================================
        // HIỂN THỊ PUBLIC PARAMETERS
        // =========================================================

        private void ShowRSAParameters(RSA key)
        {
            RSAParameters p =
                key.ExportParameters(false);

            txtParameters.Text =
                "================ PUBLIC KEY ================\r\n\r\n" +

                "n (Modulus):\r\n" +
                Convert.ToBase64String(p.Modulus!) +

                "\r\n\r\ne (Exponent):\r\n" +
                Convert.ToBase64String(p.Exponent!) +

                "\r\n\r\nKích thước khóa: 2048 bit";
        }

        // =========================================================
        // HIỂN THỊ PRIVATE PARAMETERS
        // =========================================================

        private void ShowPrivateParameters(RSA key)
        {
            RSAParameters p =
                key.ExportParameters(true);

            txtParameters.Text =
                "================ PRIVATE KEY ================\r\n\r\n" +

                "n (Modulus):\r\n" +
                Convert.ToBase64String(p.Modulus!) +

                "\r\n\r\ne (Exponent):\r\n" +
                Convert.ToBase64String(p.Exponent!) +

                "\r\n\r\nd (Private Exponent):\r\n" +
                Convert.ToBase64String(p.D!) +

                "\r\n\r\np:\r\n" +
                Convert.ToBase64String(p.P!) +

                "\r\n\r\nq:\r\n" +
                Convert.ToBase64String(p.Q!) +

                "\r\n\r\ndp:\r\n" +
                Convert.ToBase64String(p.DP!) +

                "\r\n\r\ndq:\r\n" +
                Convert.ToBase64String(p.DQ!) +

                "\r\n\r\ninverseQ:\r\n" +
                Convert.ToBase64String(p.InverseQ!);
        }

        // =========================================================
        // RSA ENCRYPT
        // =========================================================

        private byte[] EncryptRSA(
            byte[] data,
            RSA key)
        {
            // RSA-2048 + OAEP-SHA256:
            // 256 - 2*32 - 2 = 190 bytes

            if (data.Length > 190)
            {
                throw new Exception(
                    "Dữ liệu không được vượt quá 190 byte!"
                );
            }

            return key.Encrypt(
                data,
                RSAEncryptionPadding.OaepSHA256
            );
        }

        // =========================================================
        // RSA DECRYPT
        // =========================================================

        private byte[] DecryptRSA(
            byte[] encryptedData,
            RSA key)
        {
            return key.Decrypt(
                encryptedData,
                RSAEncryptionPadding.OaepSHA256
            );
        }

        // =========================================================
        // SAVE BASE64
        // =========================================================

        private void SaveEncryptedData(
            string filePath,
            byte[] encryptedData)
        {
            string base64 =
                Convert.ToBase64String(
                    encryptedData
                );

            File.WriteAllText(
                filePath,
                base64,
                Encoding.ASCII
            );
        }

        // =========================================================
        // LOAD BASE64
        // =========================================================

        private byte[] LoadEncryptedData(
            string filePath)
        {
            string base64 =
                File.ReadAllText(
                    filePath,
                    Encoding.ASCII
                ).Trim();

            return Convert.FromBase64String(
                base64
            );
        }

        // =========================================================
        // PERFORMANCE
        // =========================================================

        private void TestRSAPerformance()
        {
            if (publicKey == null ||
                privateKey == null)
            {
                MessageBox.Show(
                    "Hãy sinh hoặc import cặp khóa trước!"
                );
                return;
            }

            byte[] data =
                Encoding.UTF8.GetBytes(
                    "Du lieu kiem tra toc do RSA"
                );

            const int count = 1000;

            Stopwatch sw =
                new Stopwatch();

            byte[] encrypted = null!;

            // -------------------------
            // ENCRYPT
            // -------------------------

            sw.Start();

            for (int i = 0; i < count; i++)
            {
                encrypted =
                    publicKey.Encrypt(
                        data,
                        RSAEncryptionPadding.OaepSHA256
                    );
            }

            sw.Stop();

            double encryptMs =
                sw.Elapsed.TotalMilliseconds /
                count;

            // -------------------------
            // DECRYPT
            // -------------------------

            sw.Restart();

            for (int i = 0; i < count; i++)
            {
                privateKey.Decrypt(
                    encrypted,
                    RSAEncryptionPadding.OaepSHA256
                );
            }

            sw.Stop();

            double decryptMs =
                sw.Elapsed.TotalMilliseconds /
                count;

            txtSpeed.Text =
                "============= RSA PERFORMANCE =============\r\n\r\n" +

                $"Khóa: RSA-2048\r\n" +
                $"Padding: OAEP-SHA256\r\n" +
                $"Số lần thử: {count}\r\n\r\n" +

                $"Encrypt bằng Public Key:\r\n" +
                $"{encryptMs:F4} ms / lần\r\n\r\n" +

                $"Decrypt bằng Private Key:\r\n" +
                $"{decryptMs:F4} ms / lần\r\n\r\n" +

                "Thông thường Encrypt nhanh hơn Decrypt.";
        }

        // =========================================================
        // SHA-256
        // =========================================================

        private byte[] CalculateSHA256(
            byte[] data)
        {
            return SHA256.HashData(data);
        }

        private string SHA256Hash(
            string input)
        {
            byte[] data =
                Encoding.UTF8.GetBytes(input);

            byte[] hash =
                CalculateSHA256(data);

            return Convert.ToHexString(hash);
        }

        // =========================================================
        // ĐẾM BIT KHÁC NHAU
        // =========================================================

        private int CountDifferentBits(
            byte[] a,
            byte[] b)
        {
            if (a.Length != b.Length)
                throw new Exception(
                    "Hai hash phải có cùng kích thước!"
                );

            int count = 0;

            for (int i = 0; i < a.Length; i++)
            {
                byte xor =
                    (byte)(a[i] ^ b[i]);

                for (int j = 0; j < 8; j++)
                {
                    count +=
                        (xor >> j) & 1;
                }
            }

            return count;
        }

        // =========================================================
        // DIGITAL SIGNATURE
        // =========================================================

        private byte[] SignData(
            byte[] data,
            RSA key)
        {
            return key.SignData(
                data,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pss
            );
        }

        // =========================================================
        // VERIFY
        // =========================================================

        private bool VerifySignature(
            byte[] data,
            byte[] signature,
            RSA key)
        {
            return key.VerifyData(
                data,
                signature,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pss
            );
        }

        // =========================================================
        // BUTTON - GENERATE
        // =========================================================

        private void btnGenerateKey_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                GenerateRSAKey();

                ShowRSAParameters(
                    rsa!
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message
                );
            }
        }

        // =========================================================
        // BUTTON - EXPORT PUBLIC
        // =========================================================

        private void btnExportPublic_Click(
            object? sender,
            EventArgs e)
        {
            if (rsa == null)
            {
                MessageBox.Show(
                    "Hãy sinh khóa trước!"
                );
                return;
            }

            using SaveFileDialog dialog =
                new SaveFileDialog();

            dialog.Filter =
                "PEM file (*.pem)|*.pem";

            dialog.FileName =
                "public_key.pem";

            if (dialog.ShowDialog() ==
                DialogResult.OK)
            {
                try
                {
                    ExportPublicKeyPem(
                        dialog.FileName
                    );

                    MessageBox.Show(
                        "Đã xuất Public Key!"
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message
                    );
                }
            }
        }

        // =========================================================
        // BUTTON - EXPORT PRIVATE
        // =========================================================

        private void btnExportPrivate_Click(
            object? sender,
            EventArgs e)
        {
            if (rsa == null)
            {
                MessageBox.Show(
                    "Hãy sinh khóa trước!"
                );
                return;
            }

            using SaveFileDialog dialog =
                new SaveFileDialog();

            dialog.Filter =
                "PEM file (*.pem)|*.pem";

            dialog.FileName =
                "private_key.pem";

            if (dialog.ShowDialog() ==
                DialogResult.OK)
            {
                try
                {
                    ExportPrivateKeyPem(
                        dialog.FileName
                    );

                    MessageBox.Show(
                        "Đã xuất Private Key!"
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message
                    );
                }
            }
        }

        // =========================================================
        // BUTTON - IMPORT PUBLIC
        // =========================================================

        private void btnImportPublic_Click(
            object? sender,
            EventArgs e)
        {
            using OpenFileDialog dialog =
                new OpenFileDialog();

            dialog.Filter =
                "PEM file (*.pem)|*.pem";

            if (dialog.ShowDialog() ==
                DialogResult.OK)
            {
                try
                {
                    publicKey?.Dispose();

                    publicKey =
                        ImportPublicKey(
                            dialog.FileName
                        );

                    ShowRSAParameters(
                        publicKey
                    );

                    MessageBox.Show(
                        "Import Public Key thành công!"
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Import thất bại:\r\n" +
                        ex.Message
                    );
                }
            }
        }

        // =========================================================
        // BUTTON - IMPORT PRIVATE
        // =========================================================

        private void btnImportPrivate_Click(
            object? sender,
            EventArgs e)
        {
            using OpenFileDialog dialog =
                new OpenFileDialog();

            dialog.Filter =
                "PEM file (*.pem)|*.pem";

            if (dialog.ShowDialog() ==
                DialogResult.OK)
            {
                try
                {
                    privateKey?.Dispose();

                    privateKey =
                        ImportPrivateKey(
                            dialog.FileName
                        );

                    ShowPrivateParameters(
                        privateKey
                    );

                    MessageBox.Show(
                        "Import Private Key thành công!"
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Import thất bại:\r\n" +
                        ex.Message
                    );
                }
            }
        }

        // =========================================================
        // BUTTON - ENCRYPT
        // =========================================================

        private void btnEncrypt_Click(
            object? sender,
            EventArgs e)
        {
            if (publicKey == null)
            {
                MessageBox.Show(
                    "Hãy import Public Key!"
                );
                return;
            }

            try
            {
                byte[] data =
                    Encoding.UTF8.GetBytes(
                        txtInput.Text
                    );

                if (data.Length > 190)
                {
                    MessageBox.Show(
                        $"Dữ liệu hiện tại: {data.Length} byte.\r\n" +
                        "Giới hạn RSA-OAEP-SHA256: 190 byte."
                    );
                    return;
                }

                originalData =
                    data.ToArray();

                byte[] encrypted =
                    EncryptRSA(
                        data,
                        publicKey
                    );

                txtEncrypted.Text =
                    Convert.ToBase64String(
                        encrypted
                    );

                MessageBox.Show(
                    "Mã hóa thành công!"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Mã hóa thất bại:\r\n" +
                    ex.Message
                );
            }
        }

        // =========================================================
        // BUTTON - SAVE ENCRYPTED
        // =========================================================

        private void btnSaveEncrypted_Click(
            object? sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(
                txtEncrypted.Text))
            {
                MessageBox.Show(
                    "Chưa có dữ liệu mã hóa!"
                );
                return;
            }

            using SaveFileDialog dialog =
                new SaveFileDialog();

            dialog.Filter =
                "Text file (*.txt)|*.txt";

            dialog.FileName =
                "encrypted_data.txt";

            if (dialog.ShowDialog() ==
                DialogResult.OK)
            {
                File.WriteAllText(
                    dialog.FileName,
                    txtEncrypted.Text.Trim(),
                    Encoding.ASCII
                );

                MessageBox.Show(
                    "Đã lưu dữ liệu mã hóa!"
                );
            }
        }

        // =========================================================
        // BUTTON - LOAD ENCRYPTED
        // =========================================================

        private void btnLoadEncrypted_Click(
            object? sender,
            EventArgs e)
        {
            using OpenFileDialog dialog =
                new OpenFileDialog();

            dialog.Filter =
                "Text file (*.txt)|*.txt|All files (*.*)|*.*";

            if (dialog.ShowDialog() ==
                DialogResult.OK)
            {
                try
                {
                    byte[] encrypted =
                        LoadEncryptedData(
                            dialog.FileName
                        );

                    txtEncrypted.Text =
                        Convert.ToBase64String(
                            encrypted
                        );

                    MessageBox.Show(
                        "Đã đọc dữ liệu Base64!"
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "File không hợp lệ:\r\n" +
                        ex.Message
                    );
                }
            }
        }

        // =========================================================
        // BUTTON - DECRYPT
        // =========================================================

        private void btnDecrypt_Click(
            object? sender,
            EventArgs e)
        {
            if (privateKey == null)
            {
                MessageBox.Show(
                    "Hãy import Private Key!"
                );
                return;
            }

            try
            {
                byte[] encrypted =
                    Convert.FromBase64String(
                        txtEncrypted.Text.Trim()
                    );

                byte[] decrypted =
                    DecryptRSA(
                        encrypted,
                        privateKey
                    );

                txtDecrypted.Text =
                    Encoding.UTF8.GetString(
                        decrypted
                    );

                if (originalData != null)
                {
                    bool same =
                        originalData.SequenceEqual(
                            decrypted
                        );

                    MessageBox.Show(
                        same
                            ? "GIẢI MÃ THÀNH CÔNG!\r\nDữ liệu giống ban đầu."
                            : "Giải mã được nhưng dữ liệu khác ban đầu."
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Giải mã thành công!"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Giải mã thất bại:\r\n" +
                    ex.Message
                );
            }
        }

        // =========================================================
        // BUTTON - SPEED
        // =========================================================

        private void btnSpeed_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                TestRSAPerformance();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message
                );
            }
        }

        // =========================================================
        // BUTTON - HASH
        // =========================================================

        private void btnHash_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                string hash =
                    SHA256Hash(
                        txtHashInput.Text
                    );

                txtHash.Text =
                    "Dữ liệu:\r\n" +
                    txtHashInput.Text +

                    "\r\n\r\nSHA-256:\r\n" +
                    hash +

                    "\r\n\r\nĐộ dài:\r\n" +
                    "256 bit = 32 byte = 64 ký tự HEX";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message
                );
            }
        }

        // =========================================================
        // BUTTON - AVALANCHE
        // =========================================================

        private void btnAvalanche_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                byte[] hash1 =
                    SHA256.HashData(
                        Encoding.UTF8.GetBytes(
                            txtHashInput1.Text
                        )
                    );

                byte[] hash2 =
                    SHA256.HashData(
                        Encoding.UTF8.GetBytes(
                            txtHashInput2.Text
                        )
                    );

                int differentBits =
                    CountDifferentBits(
                        hash1,
                        hash2
                    );

                double percent =
                    differentBits /
                    256.0 *
                    100.0;

                txtHash.Text =
                    "========== AVALANCHE EFFECT ==========\r\n\r\n" +

                    "Dữ liệu 1:\r\n" +
                    txtHashInput1.Text +

                    "\r\n\r\nHash 1:\r\n" +
                    Convert.ToHexString(hash1) +

                    "\r\n\r\nDữ liệu 2:\r\n" +
                    txtHashInput2.Text +

                    "\r\n\r\nHash 2:\r\n" +
                    Convert.ToHexString(hash2) +

                    "\r\n\r\nSố bit khác nhau: " +
                    differentBits +
                    " / 256" +

                    "\r\nTỷ lệ thay đổi: " +
                    $"{percent:F2}%";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message
                );
            }
        }

        // =========================================================
        // BUTTON - SIGN
        // =========================================================

        private void btnSign_Click(
            object? sender,
            EventArgs e)
        {
            if (privateKey == null)
            {
                MessageBox.Show(
                    "Hãy import Private Key!"
                );
                return;
            }

            try
            {
                byte[] data =
                    Encoding.UTF8.GetBytes(
                        txtSignData.Text
                    );

                byte[] signature =
                    SignData(
                        data,
                        privateKey
                    );

                txtSignature.Text =
                    Convert.ToBase64String(
                        signature
                    );

                MessageBox.Show(
                    "Ký số thành công!"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ký số thất bại:\r\n" +
                    ex.Message
                );
            }
        }

        // =========================================================
        // BUTTON - COMPARE SIGNATURE
        // =========================================================

        private void btnCompareSignature_Click(
            object? sender,
            EventArgs e)
        {
            if (privateKey == null)
            {
                MessageBox.Show(
                    "Hãy import Private Key!"
                );
                return;
            }

            try
            {
                byte[] data =
                    Encoding.UTF8.GetBytes(
                        txtSignData.Text
                    );

                byte[] signature1 =
                    SignData(
                        data,
                        privateKey
                    );

                byte[] signature2 =
                    SignData(
                        data,
                        privateKey
                    );

                bool same =
                    signature1.SequenceEqual(
                        signature2
                    );

                txtSignatureCompare.Text =
                    "========== KÝ LẦN 1 ==========\r\n\r\n" +

                    Convert.ToBase64String(
                        signature1
                    ) +

                    "\r\n\r\n========== KÝ LẦN 2 ==========\r\n\r\n" +

                    Convert.ToBase64String(
                        signature2
                    ) +

                    "\r\n\r\n========== KẾT QUẢ ==========\r\n\r\n" +

                    "Hai chữ ký giống nhau: " +
                    (same ? "CÓ" : "KHÔNG") +

                    "\r\n\r\n" +

                    "Với RSA-PSS, mỗi lần ký sử dụng " +
                    "salt ngẫu nhiên nên cùng dữ liệu + " +
                    "cùng Private Key vẫn tạo ra chữ ký khác nhau.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message
                );
            }
        }

        // =========================================================
        // BUTTON - VERIFY
        // =========================================================

        private void btnVerify_Click(
            object? sender,
            EventArgs e)
        {
            if (publicKey == null)
            {
                MessageBox.Show(
                    "Hãy import Public Key!"
                );
                return;
            }

            try
            {
                byte[] data =
                    Encoding.UTF8.GetBytes(
                        txtSignData.Text
                    );

                byte[] signature =
                    Convert.FromBase64String(
                        txtSignature.Text.Trim()
                    );

                bool valid =
                    VerifySignature(
                        data,
                        signature,
                        publicKey
                    );

                MessageBox.Show(
                    valid
                        ? "CHỮ KÝ HỢP LỆ!"
                        : "CHỮ KÝ KHÔNG HỢP LỆ!"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Xác minh thất bại:\r\n" +
                    ex.Message
                );
            }
        }
    }
}