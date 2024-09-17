using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
    {
        public CustomLanguageManager()
        {
            AddTranslation("tr", "NotEmptyValidator", "'{PropertyName}' alanı boş bırakılamaz.");
            AddTranslation("tr", "EmailValidator", "'{PropertyName}' geçerli bir e-posta adresi olmalıdır.");
            AddTranslation("tr", "MinimumLengthValidator", "'{PropertyName}' en az {MinLength} karakter olmalıdır.");
            // Diğer tüm hatalar için gerekli çevirileri ekleyin
        }
    }

    // Bu ayarı Program.cs veya Startup.cs dosyasında ekleyin
   

}
