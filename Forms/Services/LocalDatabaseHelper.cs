using Forms.Models;
using System.Text.Json;

namespace Forms.Services
{
    public static class LocalDatabaseHelper
    {
        // Archivo de respuestas pendientes (compartido: las respuestas son del usuario autenticado)
        private static string FilePath => Path.Combine(FileSystem.AppDataDirectory, "pending_request.json");

        // Archivo de formularios descargados, aislado por usuario
        private static string FormsFilePath(string userId) =>
            Path.Combine(FileSystem.AppDataDirectory, $"forms_data_{userId}.json");

        // Obtiene el ID del usuario activo según la plataforma
        private static string GetCurrentUserId()
        {
            return DeviceInfo.Platform == DevicePlatform.MacCatalyst
                ? Preferences.Default.Get("user_id", "anon")
                : SecureStorage.Default.GetAsync("user_id").GetAwaiter().GetResult() ?? "anon";
        }

        public static async Task SaveResponseLocallyAsync(SubmitResponseDto newResponse)
        {
            var pendingList = new List<SubmitResponseDto>();

            //si el archivo ya existe, cargar las respuestas pendientes actuales.
            if (File.Exists(FilePath))
            {
                string json = await File.ReadAllTextAsync(FilePath);
                pendingList = JsonSerializer.Deserialize<List<SubmitResponseDto>>(json) ?? new List<SubmitResponseDto>();
            }

            //agregamos la nueva respuesta a la lista de pendientes y guardamos todo de nuevo en el archivo.
            pendingList.Add(newResponse);

            //volvemos a empaquetar todo y lo guardamos en el celular.
            string updatedJson = JsonSerializer.Serialize(pendingList);
            await File.WriteAllTextAsync(FilePath, updatedJson);
        }

        //usaremos este metodo para vaciar el archivo una vez que se envien las respuestas pendientes al servidor.
        public static void ClearPendingResponses()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }

        public static async Task SaveFormLocallyAsync(FormDto form)
        {
            string userId = GetCurrentUserId();
            string path = FormsFilePath(userId);

            var savedForms = new List<FormDto>();
            if (File.Exists(path))
            {
                string json = await File.ReadAllTextAsync(path);
                savedForms = JsonSerializer.Deserialize<List<FormDto>>(json) ?? new List<FormDto>();
            }

            savedForms.RemoveAll(f => f.IdForm == form.IdForm);
            savedForms.Add(form);

            string updateJson = JsonSerializer.Serialize(savedForms);
            await File.WriteAllTextAsync(path, updateJson);
        }

        public static async Task<FormDto> GetDownloadedFormByIdAsync(int id)
        {
            string path = FormsFilePath(GetCurrentUserId());
            if (!File.Exists(path)) return null;

            string json = await File.ReadAllTextAsync(path);
            var savedForms = JsonSerializer.Deserialize<List<FormDto>>(json) ?? new List<FormDto>();

            return savedForms.FirstOrDefault(f => f != null && f.IdForm == id);
        }

        // Elimina una encuesta descargada específica por su IdForm
        public static async Task DeleteDownloadedFormAsync(int formId)
        {
            string path = FormsFilePath(GetCurrentUserId());
            if (!File.Exists(path)) return;

            string json = await File.ReadAllTextAsync(path);
            var savedForms = JsonSerializer.Deserialize<List<FormDto>>(json) ?? new List<FormDto>();

            savedForms.RemoveAll(f => f.IdForm == formId);

            string updatedJson = JsonSerializer.Serialize(savedForms);
            await File.WriteAllTextAsync(path, updatedJson);
        }

        //Obtener todas las encuestas descargadas del usuario activo
        public static async Task<List<FormDto>> GetAllDownloadedFormAsync()
        {
            string path = FormsFilePath(GetCurrentUserId());
            if (!File.Exists(path))
                return new List<FormDto>();

            string json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<FormDto>>(json) ?? new List<FormDto>();
        }

        //Obtener todas las respuestas (Borradores) pendientes a subir
        public static async Task<List<SubmitResponseDto>> GetPendingResponsesAsync()
        {
            if (!File.Exists(FilePath))
                return new List<SubmitResponseDto>(); //Si no hay respuestas pendientes, devolvemos una lista vacía

            string json = await File.ReadAllTextAsync(FilePath);
            return JsonSerializer.Deserialize<List<SubmitResponseDto>>(json) ?? new List<SubmitResponseDto>();
        }

        public static async Task DeletePendingResponseAsync(string localId)
        {
            var pendingList = await GetPendingResponsesAsync();

            var itemToRemove = pendingList.FirstOrDefault(x => x.LocalId == localId);
            if (itemToRemove != null)
            {
                pendingList.Remove(itemToRemove);
                string updatedJson = System.Text.Json.JsonSerializer.Serialize(pendingList);
                await File.WriteAllTextAsync(FilePath, updatedJson);
            }
        }
    }
}
