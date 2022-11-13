# Scalizer for Windows 11

### [Buy me a Coffee ☕](https://www.buymeacoffee.com/wonmor)

**Scalizer** automatically configures the Windows' UI scale on unsupported displays, such as Huawei's Mateview.
It essentially *replaces* the default Windows's DPI scaling software. Served as an **Open-source** distribution; covered under the Apache 2.0 License.

---

<img width="583" alt="Screenshot 2022-11-13 104045" src="https://user-images.githubusercontent.com/35755386/201530562-6488f21f-3500-43fc-831a-f3dda39745b5.png">

<img width="574" alt="Screenshot 2022-11-13 103918" src="https://user-images.githubusercontent.com/35755386/201530569-aa2e41bf-ec6c-4c60-8b9b-a7bfadf4dbe6.png">

**IMPORTANT NOTE**: Make sure you turn on the "custom scaling factor..." on Display under Windows Settings before running this app!

---

## Dependencies
- Windows Display API
- Newtonsoft Json.NET

---

## Developer's Guide

### Display Configuration JSON File Location

e.g. Profile Name: my_profile, Display Name: MateView

```%APPLICATION_INSTALLED_FOLDER%\my_profile@MateView.json
