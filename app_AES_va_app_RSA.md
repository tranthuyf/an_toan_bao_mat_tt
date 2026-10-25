## Họ và tên: Trần Thị Thùy
## Lớp: K59KTP
## MSSV: K235480106070
# bài 1: Mã hoá AES : Mã hoá đối xứng (mã hoá, giải mã cùng dùng 1 khoá)

- tạo app (not web) : console, form

1. sinh khoá AES
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/4b0e2988-0d60-4e8f-b9af-3a3e2379e35f" />

2. lưu khoá ra file
<img width="1920" height="1080" alt="Screenshot 2026-10-25 193956" src="https://github.com/user-attachments/assets/755d8069-8076-4998-ab92-68fab0c014f5" />

3. import khoá từ file
<img width="1920" height="1080" alt="Screenshot 2026-10-25 194020" src="https://github.com/user-attachments/assets/dd1e19b1-e5df-4cf5-a039-fa4d7bea4112" />

- hiểm thị khóa
  <img width="1920" height="1080" alt="Screenshot 2026-10-25 194031" src="https://github.com/user-attachments/assets/3281af17-771f-4d6a-87e7-347632918d7f" />

4. mã hoá 1 đoạn văn bản -> kq mã hoá ra màn hình
 <img width="1920" height="1080" alt="Screenshot 2026-10-25 194139" src="https://github.com/user-attachments/assets/7c7bfd1c-208a-4ebe-84d8-bc984c8c27e4" />

5.Giai ma van bản
<img width="1920" height="1080" alt="Screenshot 2026-10-25 194202" src="https://github.com/user-attachments/assets/26cff863-159d-473d-b4fa-272ecafc704e" />

6. Mã hóa file
<img width="1920" height="1080" alt="Screenshot 2026-10-25 194306" src="https://github.com/user-attachments/assets/d1ebe36a-ac72-4c18-9fa7-b1f76f5c5554" />

7. Giải mã file
<img width="1920" height="1080" alt="Screenshot 2026-10-25 194323" src="https://github.com/user-attachments/assets/a93a05d3-ed69-42ae-98b0-a8a0e2efbae7" />

8. So sánh 2 file
<img width="1920" height="1080" alt="Screenshot 2026-10-25 194339" src="https://github.com/user-attachments/assets/2f2bea54-c02e-4d93-8c90-bd976026dbdb" />

<img width="1920" height="1080" alt="Screenshot 2026-10-25 194355" src="https://github.com/user-attachments/assets/cf42261d-9e47-4e00-a6a1-822436127d7f" />

# bài tập 2: mã hoá RSA : mã hoá bất đối xứng (mã hoá 1 chìa, giải mã bằng chìa còn lại)
 1. sinh cặp khoá
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/92e41748-fcd5-4f28-9bd0-a12f40f97526" />

2. xuất cặp khoá ra file (pem)
- Xuất file public
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/32b32afc-1afa-4a48-9d30-6080288c05f9" />

- Xuất file Private
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/6d79df38-7f09-4273-bde0-31423ca8f205" />

- sau khi xuất ra file sẽ lưu trong tệp
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/df1ac41b-5adb-4efd-80a9-73e3edd26f04" />

3. nhập 1 file pem : public key được n và e

    nhập 1 file pem : priavte key được n và e, d, ...

- import file public
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/f1d2f693-6ee4-4e81-8a50-8fac520d29ba" />
 <img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/0fdcb02b-d7cc-45a0-b947-130588b973bc" />

- import file private
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/047908f3-eacb-4f76-b53d-b22047a8e1c7" />
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/db81efae-0df4-49c7-8f89-5819f403f91b" />

4. mã hoá dữ liệu sử dụng key vừa import:
   
    dữ liệu là 1 mảng byte ko quá 190 byte
   (khoá AES 256 bit <=> 32 byte (base64))
   
   kq mã hoá đc mảng byte (lưu ở dạng base64) => lưu file
   
-  mã hoá dữ liệu sử dụng key vừa import:
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/742286e8-856f-496c-b099-541ac0239bac" />
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/81041798-d3c0-460c-bb88-046b02fe0583" />


5. giải mã: đầu vào là pem, file chứa data đã mã hoá (base64)
   so sánh kq giải mã với nguồn trước khi mã hoá.

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/13a45ccd-ae55-4684-ba8d-a9ad70203bf1" />

6. viết đoạn code đánh giá tốc độ mã hoá, giải mã 
   xem dùng khoá nào (pulic key: n, e bé), private key(n, d to đùng)
   thì tốc độ ra sao?
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/c2c9ce35-d2ed-4a7d-83e8-20ae29a0c4b6" />

7. Tạo giao diện sinh chuỗi băm SHA256 bít

   khi thay đổi đầu vào => băm mới: đánh giá sự thay đổi của kq băm  khi dữ liệu đầu vào thay đổi (rất ít)
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/c4a948f9-726e-410a-9a72-cc558cc28d4a" />

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/ca61d8f5-64ed-43b3-b948-ff36520a2cb2" />

8. CHỮ KÝ SỐ: thực ra là dùng RSA với khoá bí mật cho kq băm của dữ liệu => được gọi là chữ ký số của dữ liệu đó.

=> dữ liệu khác nhau : có chữ ký số khác nhau (của cùng người ký)

- Ký lần một
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/e633263d-194b-4ed6-b372-808e27916e90" />

- Ký lần hai
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/d286e6c6-ecad-4e61-945f-ba2484bda121" />

- So sánh 2 lần ký
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/0300b3c1-28eb-48fa-affe-0b7faf179b3f" />
