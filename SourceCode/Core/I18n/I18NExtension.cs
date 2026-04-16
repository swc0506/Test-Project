namespace Core.I18N
{
    public static class I18NExtension
    {
        public static string ToI18N(this string key, params object[] args)
        {
            return LocalizationManager.Instance.GetText(key, args);
        }
    }
}