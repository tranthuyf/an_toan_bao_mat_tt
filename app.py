from flask import Flask, render_template, request
from Crypto.Cipher import AES
import base64


app = Flask(__name__)


# =========================
# HÀM PAD DỮ LIỆU
# =========================
def pad(data):
    padding_length = 16 - (len(data) % 16)
    return data + bytes([padding_length]) * padding_length


# =========================
# HÀM UNPAD DỮ LIỆU
# =========================
def unpad(data):
    padding_length = data[-1]
    return data[:-padding_length]


# =========================
# MÃ HÓA AES
# =========================
def encrypt_aes(plaintext, key):
    cipher = AES.new(key, AES.MODE_ECB)

    plaintext_bytes = plaintext.encode("utf-8")

    padded_data = pad(plaintext_bytes)

    ciphertext = cipher.encrypt(padded_data)

    return base64.b64encode(ciphertext).decode("utf-8")


# =========================
# GIẢI MÃ AES
# =========================
def decrypt_aes(ciphertext, key):
    cipher = AES.new(key, AES.MODE_ECB)

    ciphertext_bytes = base64.b64decode(ciphertext)

    decrypted_data = cipher.decrypt(ciphertext_bytes)

    plaintext = unpad(decrypted_data)

    return plaintext.decode("utf-8")


# =========================
# TRANG CHỦ
# =========================
@app.route("/", methods=["GET", "POST"])
def index():

    result = ""
    error = ""
    mode = ""

    if request.method == "POST":

        mode = request.form.get("mode")
        key_text = request.form.get("key", "")

        # Kiểm tra khóa
        if len(key_text) not in [16, 24, 32]:

            error = (
                "Khóa AES phải có 16, 24 hoặc 32 ký tự "
                "(AES-128, AES-192 hoặc AES-256)."
            )

        else:

            key = key_text.encode("utf-8")

            try:

                if mode == "encrypt":

                    plaintext = request.form.get(
                        "plaintext", ""
                    )

                    if plaintext == "":
                        error = "Vui lòng nhập bản rõ."

                    else:
                        result = encrypt_aes(
                            plaintext,
                            key
                        )

                elif mode == "decrypt":

                    ciphertext = request.form.get(
                        "ciphertext", ""
                    )

                    if ciphertext == "":
                        error = "Vui lòng nhập bản mã."

                    else:
                        result = decrypt_aes(
                            ciphertext,
                            key
                        )

            except Exception:
                error = (
                    "Không thể xử lý dữ liệu. "
                    "Hãy kiểm tra khóa và bản mã."
                )

    return render_template(
        "index.html",
        result=result,
        error=error,
        mode=mode
    )


if __name__ == "__main__":
    app.run(debug=True)