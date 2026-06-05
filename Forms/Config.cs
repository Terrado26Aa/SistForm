namespace Forms
{
    public static class Config
    {
        // Centraliza la URL de la API para facilitar cambios entre entornos
        // 10.0.2.2 es el alias para localhost en el emulador de Android
        public static string BaseApiUrl = DeviceInfo.Platform == DevicePlatform.Android 
            ? "http://10.0.2.2:5174" 
            : "http://127.0.0.1:5174";

        // Nota: Para dispositivos iOS físicos, cambia localhost por la IP de tu máquina
    }
}
