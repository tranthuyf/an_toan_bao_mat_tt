from Crypto.Cipher import AES
from Crypto.Random import get_random_bytes
import base64


# Hàm mã hóa AES
def encrypt_aes(plaintext, key):
    cipher = AES.new(key, AES.MODE_EAX)

    ciphertext, tag = cipher.encrypt_and_digest(
        plaintext.encode("utf-8")
    )

    return (
        cipher.nonce,
        ciphertext,
        tag
    )


# Hàm giải mã AES
def decrypt_aes(nonce, ciphertext, tag, key):
    cipher = AES.new(
        key,
        AES.MODE_EAX,
        nonce=nonce
    )

    plaintext = cipher.decrypt_and_verify(
        ciphertext,
        tag
    )

    return plaintext.decode("utf-8")


# =========================
# CHƯƠNG TRÌNH CHÍNH
# =========================

# Tạo khóa AES 128 bit
key = get_random_bytes(16)

# Dữ liệu ban đầu
plaintext = "Xin chao, day la du lieu bi mat!"

print("===================================")
print("      CHUONG TRINH MA HOA AES")
print("===================================")

print("\nBan ro:")
print(plaintext)


# =========================
# MÃ HÓA
# =========================

nonce, ciphertext, tag = encrypt_aes(
    plaintext,
    key
)

print("\n---------- MA HOA ----------")

print("Khoa AES:")
print(base64.b64encode(key).decode())

print("Nonce:")
print(base64.b64encode(nonce).decode())

print("Ban ma:")
print(base64.b64encode(ciphertext).decode())

print("Authentication Tag:")
print(base64.b64encode(tag).decode())


# =========================
# GIẢI MÃ
# =========================

decrypted_text = decrypt_aes(
    nonce,
    ciphertext,
    tag,
    key
)

print("\n---------- GIAI MA ----------")

print("Ban ro sau khi giai ma:")
print(decrypted_text)

print("\n===================================")
print("       HOAN THANH")
print("===================================")