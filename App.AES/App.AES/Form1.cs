using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace App.AES
{
    public partial class Form1 : Form
    {
        // Khóa AES hiện tại
        private byte[] aesKey;

        // Kích thước mỗi block đọc file
        private const int BUFFER_SIZE = 1024 * 1024; // 1 MB

        public Form1()
        {
            InitializeComponent();
        }

        // =========================================================
        // 1. SINH KHÓA AES-256
        // =========================================================
        private void GenerateKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();

                aesKey = aes.Key;
            }
        }

        // =========================================================
        // 2. LƯU KHÓA RA FILE
        // =========================================================
        private void SaveKeyToFile(string filePath)
        {
            if (aesKey == null)
                throw new Exception("Chưa có khóa AES.");

            string keyBase64 =
                Convert.ToBase64String(aesKey);

            File.WriteAllText(
                filePath,
                keyBase64,
                Encoding.UTF8);
        }

        // =========================================================
        // 3. IMPORT KHÓA TỪ FILE
        // =========================================================
        private void ImportKeyFromFile(string filePath)
        {
            string keyBase64 =
                File.ReadAllText(
                    filePath,
                    Encoding.UTF8).Trim();

            byte[] key =
                Convert.FromBase64String(keyBase64);

            if (key.Length != 32)
            {
                throw new Exception(
                    "Khóa không hợp lệ. " +
                    "Phải là khóa AES-256.");
            }

            aesKey = key;
        }

        // =========================================================
        // 4. MÃ HÓA VĂN BẢN
        // =========================================================
        private string EncryptText(string plainText)
        {
            if (aesKey == null)
                throw new Exception("Chưa có khóa AES.");

            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;

                // Sinh IV ngẫu nhiên
                aes.GenerateIV();

                byte[] plainBytes =
                    Encoding.UTF8.GetBytes(plainText);

                using (MemoryStream ms =
                    new MemoryStream())
                {
                    // Lưu IV ở đầu dữ liệu
                    ms.Write(
                        aes.IV,
                        0,
                        aes.IV.Length);

                    using (CryptoStream cs =
                        new CryptoStream(
                            ms,
                            aes.CreateEncryptor(),
                            CryptoStreamMode.Write))
                    {
                        cs.Write(
                            plainBytes,
                            0,
                            plainBytes.Length);

                        cs.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(
                        ms.ToArray());
                }
            }
        }

        // =========================================================
        // 5. GIẢI MÃ VĂN BẢN
        // =========================================================
        private string DecryptText(string encryptedText)
        {
            if (aesKey == null)
                throw new Exception("Chưa có khóa AES.");

            byte[] encryptedBytes =
                Convert.FromBase64String(
                    encryptedText);

            if (encryptedBytes.Length < 16)
                throw new Exception(
                    "Dữ liệu mã hóa không hợp lệ.");

            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;

                // Lấy IV ở 16 byte đầu
                byte[] iv = new byte[16];

                Array.Copy(
                    encryptedBytes,
                    0,
                    iv,
                    0,
                    16);

                aes.IV = iv;

                using (MemoryStream ms =
                    new MemoryStream())
                {
                    using (CryptoStream cs =
                        new CryptoStream(
                            ms,
                            aes.CreateDecryptor(),
                            CryptoStreamMode.Write))
                    {
                        cs.Write(
                            encryptedBytes,
                            16,
                            encryptedBytes.Length - 16);

                        cs.FlushFinalBlock();
                    }

                    return Encoding.UTF8.GetString(
                        ms.ToArray());
                }
            }
        }

        // =========================================================
        // 6. MÃ HÓA FILE
        //
        // Cấu trúc file mã hóa:
        //
        // [16 byte IV]
        // [dữ liệu đã mã hóa]
        //
        // =========================================================
        private void EncryptFile(
            string inputFile,
            string outputFile)
        {
            if (aesKey == null)
                throw new Exception(
                    "Chưa có khóa AES.");

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Key = aesKey;

                // Sinh IV
                aes.GenerateIV();

                using (FileStream input =
                    new FileStream(
                        inputFile,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        BUFFER_SIZE,
                        FileOptions.SequentialScan))
                {
                    using (FileStream output =
                        new FileStream(
                            outputFile,
                            FileMode.Create,
                            FileAccess.Write,
                            FileShare.None,
                            BUFFER_SIZE,
                            FileOptions.SequentialScan))
                    {
                        // Ghi IV vào đầu file
                        output.Write(
                            aes.IV,
                            0,
                            aes.IV.Length);

                        using (CryptoStream cryptoStream =
                            new CryptoStream(
                                output,
                                aes.CreateEncryptor(),
                                CryptoStreamMode.Write))
                        {
                            byte[] buffer =
                                new byte[BUFFER_SIZE];

                            int bytesRead;

                            while ((bytesRead =
                                input.Read(
                                    buffer,
                                    0,
                                    buffer.Length)) > 0)
                            {
                                cryptoStream.Write(
                                    buffer,
                                    0,
                                    bytesRead);
                            }

                            cryptoStream.FlushFinalBlock();
                        }
                    }
                }
            }
        }

        // =========================================================
        // 7. GIẢI MÃ FILE
        // =========================================================
        private void DecryptFile(
            string inputFile,
            string outputFile)
        {
            if (aesKey == null)
                throw new Exception(
                    "Chưa có khóa AES.");

            using (FileStream input =
                new FileStream(
                    inputFile,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    BUFFER_SIZE,
                    FileOptions.SequentialScan))
            {
                // Đọc IV 16 byte đầu
                byte[] iv = new byte[16];

                int totalRead = 0;

                while (totalRead < 16)
                {
                    int read = input.Read(
                        iv,
                        totalRead,
                        16 - totalRead);

                    if (read == 0)
                        throw new Exception(
                            "File mã hóa không hợp lệ.");

                    totalRead += read;
                }

                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.Key = aesKey;
                    aes.IV = iv;

                    using (FileStream output =
                        new FileStream(
                            outputFile,
                            FileMode.Create,
                            FileAccess.Write,
                            FileShare.None,
                            BUFFER_SIZE,
                            FileOptions.SequentialScan))
                    {
                        using (CryptoStream cryptoStream =
                            new CryptoStream(
                                input,
                                aes.CreateDecryptor(),
                                CryptoStreamMode.Read))
                        {
                            byte[] buffer =
                                new byte[BUFFER_SIZE];

                            int bytesRead;

                            while ((bytesRead =
                                cryptoStream.Read(
                                    buffer,
                                    0,
                                    buffer.Length)) > 0)
                            {
                                output.Write(
                                    buffer,
                                    0,
                                    bytesRead);
                            }
                        }
                    }
                }
            }
        }

        // =========================================================
        // 8. SO SÁNH HAI FILE THEO BYTE
        // =========================================================
        private bool CompareFiles(
            string file1,
            string file2)
        {
            FileInfo info1 =
                new FileInfo(file1);

            FileInfo info2 =
                new FileInfo(file2);

            // Nếu kích thước khác nhau
            // thì chắc chắn không giống nhau
            if (info1.Length != info2.Length)
                return false;

            using (FileStream fs1 =
                new FileStream(
                    file1,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    BUFFER_SIZE,
                    FileOptions.SequentialScan))
            {
                using (FileStream fs2 =
                    new FileStream(
                        file2,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        BUFFER_SIZE,
                        FileOptions.SequentialScan))
                {
                    byte[] buffer1 =
                        new byte[BUFFER_SIZE];

                    byte[] buffer2 =
                        new byte[BUFFER_SIZE];

                    int bytesRead1;
                    int bytesRead2;

                    do
                    {
                        bytesRead1 =
                            fs1.Read(
                                buffer1,
                                0,
                                buffer1.Length);

                        bytesRead2 =
                            fs2.Read(
                                buffer2,
                                0,
                                buffer2.Length);

                        if (bytesRead1 != bytesRead2)
                            return false;

                        for (int i = 0;
                             i < bytesRead1;
                             i++)
                        {
                            if (buffer1[i] != buffer2[i])
                                return false;
                        }

                    } while (bytesRead1 > 0);
                }
            }

            return true;
        }

        // =========================================================
        // 9. FORMAT THỜI GIAN
        // =========================================================
        private string FormatTime(long milliseconds)
        {
            return milliseconds + " ms";
        }

        // =========================================================
        // 10. NÚT SINH KHÓA
        // =========================================================
        private void btnGenerateKey_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                GenerateKey();

                txtKey.Text =
                    Convert.ToBase64String(
                        aesKey);

                MessageBox.Show(
                    "Đã sinh khóa AES-256!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi: " + ex.Message);
            }
        }

        // =========================================================
        // 11. NÚT LƯU KHÓA
        // =========================================================
        private void btnSaveKey_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (aesKey == null)
                {
                    MessageBox.Show(
                        "Chưa có khóa. " +
                        "Hãy sinh khóa trước.");
                    return;
                }

                using (SaveFileDialog dialog =
                    new SaveFileDialog())
                {
                    dialog.Filter =
                        "Text file (*.txt)|*.txt";

                    dialog.FileName =
                        "aes_key.txt";

                    if (dialog.ShowDialog() ==
                        DialogResult.OK)
                    {
                        SaveKeyToFile(
                            dialog.FileName);

                        MessageBox.Show(
                            "Đã lưu khóa thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi: " + ex.Message);
            }
        }

        // =========================================================
        // 12. NÚT IMPORT KHÓA
        // =========================================================
        private void btnImportKey_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                using (OpenFileDialog dialog =
                    new OpenFileDialog())
                {
                    dialog.Filter =
                        "Text file (*.txt)|*.txt";

                    if (dialog.ShowDialog() ==
                        DialogResult.OK)
                    {
                        ImportKeyFromFile(
                            dialog.FileName);

                        txtKey.Text =
                            Convert.ToBase64String(
                                aesKey);

                        MessageBox.Show(
                            "Import khóa thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể import khóa.\n\n" +
                    ex.Message);
            }
        }

        // =========================================================
        // 13. NÚT MÃ HÓA VĂN BẢN
        // =========================================================
        private void btnEncryptText_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (aesKey == null)
                {
                    MessageBox.Show(
                        "Chưa có khóa AES.");
                    return;
                }

                if (string.IsNullOrEmpty(
                    txtPlainText.Text))
                {
                    MessageBox.Show(
                        "Vui lòng nhập văn bản.");
                    return;
                }

                string result =
                    EncryptText(
                        txtPlainText.Text);

                txtEncryptedText.Text =
                    result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi mã hóa: " +
                    ex.Message);
            }
        }

        // =========================================================
        // 14. NÚT GIẢI MÃ VĂN BẢN
        // =========================================================
        private void btnDecryptText_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (aesKey == null)
                {
                    MessageBox.Show(
                        "Chưa có khóa AES.");
                    return;
                }

                if (string.IsNullOrEmpty(
                    txtEncryptedText.Text))
                {
                    MessageBox.Show(
                        "Chưa có dữ liệu mã hóa.");
                    return;
                }

                string result =
                    DecryptText(
                        txtEncryptedText.Text);

                txtPlainText.Text =
                    result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi giải mã: " +
                    ex.Message);
            }
        }

        // =========================================================
        // 15. NÚT MÃ HÓA FILE
        // =========================================================
        private void btnEncryptFile_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (aesKey == null)
                {
                    MessageBox.Show(
                        "Chưa có khóa AES.");
                    return;
                }

                using (OpenFileDialog openDialog =
                    new OpenFileDialog())
                {
                    openDialog.Title =
                        "Chọn file cần mã hóa";

                    if (openDialog.ShowDialog() !=
                        DialogResult.OK)
                        return;

                    string inputFile =
                        openDialog.FileName;

                    using (SaveFileDialog saveDialog =
                        new SaveFileDialog())
                    {
                        saveDialog.Title =
                            "Lưu file đã mã hóa";

                        saveDialog.FileName =
                            Path.GetFileName(
                                inputFile) +
                            ".aes";

                        if (saveDialog.ShowDialog() !=
                            DialogResult.OK)
                            return;

                        string outputFile =
                            saveDialog.FileName;

                        FileInfo info =
                            new FileInfo(inputFile);

                        Stopwatch stopwatch =
                            Stopwatch.StartNew();

                        EncryptFile(
                            inputFile,
                            outputFile);

                        stopwatch.Stop();

                        double seconds =
                            stopwatch.Elapsed.TotalSeconds;

                        double speedMB =
                            seconds > 0
                            ? info.Length /
                              1024.0 /
                              1024.0 /
                              seconds
                            : 0;

                        txtResult.Text =
                            "MÃ HÓA FILE THÀNH CÔNG\n\n" +
                            "File nguồn:\n" +
                            inputFile + "\n\n" +
                            "File mã hóa:\n" +
                            outputFile + "\n\n" +
                            "Kích thước: " +
                            info.Length.ToString(
                                "N0") +
                            " bytes\n\n" +
                            "Thời gian mã hóa: " +
                            stopwatch.ElapsedMilliseconds +
                            " ms\n\n" +
                            "Tốc độ: " +
                            speedMB.ToString("F2") +
                            " MB/s";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi mã hóa file:\n" +
                    ex.Message);
            }
        }

        // =========================================================
        // 16. NÚT GIẢI MÃ FILE
        // =========================================================
        private void btnDecryptFile_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (aesKey == null)
                {
                    MessageBox.Show(
                        "Chưa có khóa AES.");
                    return;
                }

                using (OpenFileDialog openDialog =
                    new OpenFileDialog())
                {
                    openDialog.Title =
                        "Chọn file AES cần giải mã";

                    openDialog.Filter =
                        "AES file (*.aes)|*.aes|All files (*.*)|*.*";

                    if (openDialog.ShowDialog() !=
                        DialogResult.OK)
                        return;

                    string inputFile =
                        openDialog.FileName;

                    using (SaveFileDialog saveDialog =
                        new SaveFileDialog())
                    {
                        saveDialog.Title =
                            "Lưu file sau khi giải mã";

                        string originalName =
                            Path.GetFileName(
                                inputFile);

                        if (originalName.EndsWith(
                            ".aes",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            originalName =
                                originalName.Substring(
                                    0,
                                    originalName.Length - 4);
                        }

                        saveDialog.FileName =
                            "decrypted_" +
                            originalName;

                        if (saveDialog.ShowDialog() !=
                            DialogResult.OK)
                            return;

                        string outputFile =
                            saveDialog.FileName;

                        Stopwatch stopwatch =
                            Stopwatch.StartNew();

                        DecryptFile(
                            inputFile,
                            outputFile);

                        stopwatch.Stop();

                        FileInfo info =
                            new FileInfo(outputFile);

                        double seconds =
                            stopwatch.Elapsed.TotalSeconds;

                        double speedMB =
                            seconds > 0
                            ? info.Length /
                              1024.0 /
                              1024.0 /
                              seconds
                            : 0;

                        txtResult.Text =
                            "GIẢI MÃ FILE THÀNH CÔNG\n\n" +
                            "File mã hóa:\n" +
                            inputFile + "\n\n" +
                            "File sau giải mã:\n" +
                            outputFile + "\n\n" +
                            "Kích thước: " +
                            info.Length.ToString(
                                "N0") +
                            " bytes\n\n" +
                            "Thời gian giải mã: " +
                            stopwatch.ElapsedMilliseconds +
                            " ms\n\n" +
                            "Tốc độ: " +
                            speedMB.ToString("F2") +
                            " MB/s";

                        // So sánh file
                        // Nếu có thể xác định file gốc
                        // thì người dùng có thể tự chọn
                        // để kiểm tra bên dưới.
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi giải mã file:\n" +
                    ex.Message);
            }
        }
    }
}