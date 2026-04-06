using Forms.Models;
using System.Text.Json;

namespace Forms.Services
{
    class LocalDatabaseHelper
    {
        //ruta segura y privada para almacenar los datos de las respuestas pendientes
        private static string FilePath => Path.Combine(FileSystem.AppDataDirectory, "pending_request.json");

        //Guardar una encuesta en el celular.
        private static string FormsFilePath => Path.Combine(FileSystem.AppDataDirectory, "forms_data.json");

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
            var savedForms = new List<FormDto>();
            if(File.Exists(FormsFilePath))
            {
                string json = await File.ReadAllTextAsync(FormsFilePath);
                savedForms = JsonSerializer.Deserialize<List<FormDto>>(json) ?? new List<FormDto>();
            }

            //Si la encuesta ya esta descargada, la eliminamos para reemplazarla por la nueva version.
            savedForms.RemoveAll(f => f.IdForm == form.IdForm);

            //agregamos la nueva version.
            savedForms.Add(form);

            string updateJson = JsonSerializer.Serialize(savedForms);
            await File.WriteAllTextAsync(FormsFilePath, updateJson);
        }

        public static async Task<FormDto> GetDownloadedFormByIdAsync(int id)
        {
            if (!File.Exists(FormsFilePath)) return null;

            string json = await File.ReadAllTextAsync(FormsFilePath);
            var savedForms = JsonSerializer.Deserialize<List<FormDto>>(json) ?? new List<FormDto>();

            return savedForms.FirstOrDefault(f => f != null && f.IdForm == id);
        }
        
        //Obtener todas las encuestad descargadas
        public static async Task<List<FormDto>> GetAllDownloadedFormAsync()
        {
            if (!File.Exists(FormsFilePath))
                return new List<FormDto>(); //Sino hay archivos devolvemos una lista vacia

            string json = await File.ReadAllTextAsync(FormsFilePath);
            return System.Text.Json.JsonSerializer.Deserialize<List<FormDto>>(json) ?? new List<FormDto>();
        }
    }
}
