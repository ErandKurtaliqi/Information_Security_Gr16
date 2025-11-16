# Blowfish Decryption API – Dokumentacioni i Projektit

Ky projekt implementon dekriptimin e tekstit duke përdorur algoritmin **Blowfish** në .NET. Përdor **CBC mode**, **PKCS7 padding**, çelësa në **Base64**, dhe **IV në HEX**. Ofron endpoint-e REST për enkriptim dhe dekriptim, validime të forta dhe konfigurim fleksibël.

---

## Përmbajtja

* Përshkrimi i Përgjithshëm
* Konfigurimi
* BlowfishService
* API Controller
* Modelet
* Përdorimi i API
* Validimet dhe Siguria
* Regjistrimi i Shërbimit
* Shembull i appsettings.json
* Përfundim

---

## Përshkrimi i Përgjithshëm

Ky projekt paraqet një implementim të plotë dhe profesional të algoritmit Blowfish për enkriptim dhe dekriptim të të dhënave brenda një API-je të zhvilluar me ASP.NET Core (.NET 8). Blowfish është një algoritëm simetrik i bllokut, i njohur për shpejtësinë dhe thjeshtësinë e tij, si dhe për fleksibilitetin në madhësinë e çelësave (deri në 448 bit).

Ky implementim fokusohet në ofrimin e një shërbimi të sigurt, të konfigurueshëm dhe praktik për aplikacionet që kërkojnë mbrojtje të të dhënave përmes enkriptimit. Përmes këtij projekti, është krijuar një API moderne që lejon:

### Enkriptim Blowfish

API-ja konverton çdo plaintext në ciphertext duke përdorur:

- **CBC (Cipher Block Chaining) mode** – rrit sigurinë duke përdorur një IV unik për çdo enkriptim.
- **PKCS7 Padding** – garanton që ciphertext të jetë gjithmonë i plotë sa i përket madhësisë së bllokut.
- **Çelësa dinamikë ose statikë** – përdoruesi mund të dërgojë key & IV manualisht ose mund të përdoren ato të përcaktuara në konfigurim.

### Dekriptim Blowfish

API-ja mundëson rikthimin e ciphertext-it (në Base64) në tekst të lexueshëm, duke përdorur:

- Të njëjtin çelës dhe IV që janë përdorur gjatë enkriptimit
- Kontroll dhe trajtim të gabimeve për rastet kur çelësi është i pasaktë ose ciphertext është korruptuar

### Konfigurim me IOptions

Projekti përdor mekanizmin e integruar të ASP.NET Core për:

- leximin e konfigurimeve nga `appsettings.json`
- injektimin e tyre te shërbimet
- ruajtjen e `KeyBase64` dhe `IVHex` në mënyrë të sigurt

Kjo bën të mundur:

- ndryshimin e çelësave pa modifikuar kodin  
- përdorimin e konfigurimeve të ndryshme për Development, Staging, Production

### API REST

Projekti ekspozon dy endpoint-e kryesore:

#### POST `/api/blowfish/encrypt`
Pranon tekst të thjeshtë dhe kthen ciphertext të enkriptuar në Base64.

#### POST `/api/blowfish/decrypt`
Pranon ciphertext të koduar në Base64 dhe e kthen në plaintext.

**Karakteristikat e përbashkëta:**

- mbështesin parametrat opsionalë (`keyBase64`, `ivHex`)
- përdorin çelësat nga `appsettings.json` nëse nuk dërgohen në kërkesë
- kthejnë përgjigje të standardizuara (`CryptoResponseModel`)

### Mbështetje për Çelësa Statikë ose Dinamikë

Përdoruesi ka fleksibilitet të plotë:

- **Çelësa statikë** – të konfiguruar paraprakisht dhe të njëjtë për çdo kërkesë  
- **Çelësa dinamikë** – dërgohen për çdo kërkesë në JSON

E përshtatshme për:

- sisteme enkriptimi brenda aplikacioneve ekzistuese
- integrime me shërbime të jashtme
- testime me çelësa të ndryshëm

### Validime dhe Error Handling i Integruar

Projekti përmban:

- validim të input-eve me `[Required]`
- validim të formateve (Base64, HEX, madhësia e çelësit)
- trajtim të gabimeve përmes `Problem(...)`
- mesazhe të qarta gabimesh për përdoruesit

---

## Konfigurimi

```csharp
public class BlowfishOptions
{
    public string KeyBase64 { get; set; } = string.Empty;
    public string IVHex { get; set; } = string.Empty;
}
```

Këto vlera lexohen përmes `IOptions<BlowfishOptions>`.

---

## BlowfishService

`BlowfishService` përmban implementimin e algoritmit dhe përdor:

* `BlowfishEngine`
* `CbcBlockCipher`
* `Pkcs7Padding`
* `PaddedBufferedBlockCipher`

### Inicializimi

* Merr `KeyBase64` dhe `IVHex` nga konfigurimi
* Base64 → `byte[]`
* HEX → `byte[]`

### Validime

* IV = **8 bajte**
* Çelësi = **4–56 bajte**

### Metoda

```csharp
Task<string> EncryptToBase64Async(string plaintext);
Task<string> DecryptFromBase64Async(string base64Cipher);
Task<string> EncryptToBase64Async(string plaintext, byte[] key, byte[] iv8);
Task<string> DecryptFromBase64Async(string base64Cipher, byte[] key, byte[] iv8);
```

Metodat pa parametra përdorin vlerat në konfigurim.

---

## API Controller

`BlowfishController` përmban dy endpoint-e:

### `POST /api/blowfish/encrypt`

* Enkripton plaintext → Base64.

### `POST /api/blowfish/decrypt`

* Dekripton Base64 ciphertext → plaintext.

Nëse në kërkesë nuk dërgohet `KeyBase64` dhe `IVHex`, përdoren ato nga konfigurimi.

---

## Modelet

### EncryptRequestModel

```csharp
public sealed class EncryptRequestModel
{
    [Required] public string Plaintext { get; set; } = "";
    public string? KeyBase64 { get; set; }
    public string? IVHex { get; set; }
}
```

### DecryptRequestModel

```csharp
public sealed class DecryptRequestModel
{
    [Required] public string CipherBase64 { get; set; } = "";
    public string? KeyBase64 { get; set; }
    public string? IVHex { get; set; }
}
```

### CryptoResponseModel

```csharp
public class CryptoResponseModel
{
    public string Result { get; init; } = "";
}
```

---

## Përdorimi i API

### Enkriptimi

**POST /api/blowfish/encrypt**

#### Body minimal

```json
{
  "plaintext": "Pershendetje Blowfish!"
}
```

#### Body me key & IV custom

```json
{
  "plaintext": "Pershendetje Blowfish!",
  "keyBase64": "BASE64_KEY_KETU",
  "ivHex": "0123456789ABCDEF"
}
```

#### Përgjigje

```json
{
  "result": "BASE64_CIPHER_KETU"
}
```

---

### Dekriptimi

**POST /api/blowfish/decrypt**

#### Body minimal

```json
{
  "cipherBase64": "BASE64_CIPHER_KETU"
}
```

#### Body me çelësa custom

```json
{
  "cipherBase64": "BASE64_CIPHER_KETU",
  "keyBase64": "BASE64_KEY_KETU",
  "ivHex": "0123456789ABCDEF"
}
```

#### Përgjigje

```json
{
  "result": "Pershendetje Blowfish!"
}
```

---

## Validimet dhe Siguria

* **IV = 8 bajte** (16 HEX chars)
* **Çelësi Blowfish = 4–56 bajte**
* `[Required]` kontrollon input-in
* Gabimet menaxhohen me:
  `return Problem("Encrypt error: ...");`

---

## Regjistrimi i Shërbimit (Program.cs)

```csharp
builder.Services.Configure<BlowfishOptions>(
    builder.Configuration.GetSection("Blowfish"));

builder.Services.AddScoped<IBlowfishService, BlowfishService>();
```

---

## Shembull i appsettings.json

```json
{
  "Blowfish": {
    "KeyBase64": "your_base64_key_here",
    "IVHex": "0123456789ABCDEF"
  }
}
```

---

## Struktura e Projektit

<img width="384" height="535" alt="image" src="https://github.com/user-attachments/assets/21889f3f-b698-42e8-8109-0bed7a3042a1" />

---

## Testimi përmes Swagger

<img width="1194" height="864" alt="image" src="https://github.com/user-attachments/assets/e3521156-33ee-423e-87de-688c61381b9a" />

Swagger auto–gjenerohet nga ASP.NET Core dhe ofron testimin e drejtpërdrejtë të endpoint-eve.

<img width="1103" height="919" alt="image" src="https://github.com/user-attachments/assets/722f5b90-2a2c-4524-9b3a-a0d58d587170" />

Për të dekriptuar një tekst si në foto ne duhet ta kemi keyBase64 dhe ivHex, por pasi që  kemi enkriptuar pa këto paraprakisht të cakuara, programi siguron një keyBase64 dhe një ivHex, me të cilat enkriptojm dhe pastaj kemi përdorur të njëjtat për dekriptim.

### Shembull – Dekriptimi (`POST /api/blowfish/decrypt`)

**Body minimal:**

```json
{
  "cipherBase64": "BASE64_CIPHER_KETU"
}
```

**Përgjigja:**

```json
{
  "result": "Teksti i dekriptuar këtu"
}
```

---

## Përfundim

Ky projekt ofron:

* Dekriptim Blowfish në mënyrë profesionale
* API të thjeshtë dhe të sigurt
* Mbështetje për çelësa statikë dhe dinamikë
* Validime të forta dhe error–handling korrekt
* Kod të pastër dhe të mirëstrukturuar, me mundësi zgjerimi në të ardhmen
