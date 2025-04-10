﻿using Newtonsoft.Json;

namespace MvcCubos.Extensions {
    public static class SessionExtension
    {
        // Método para guardar un objeto en la sesión
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // Método para recuperar un objeto desde la sesión
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
