namespace KinectDemo2.Custom.Helper.Settings
{
    internal interface IWritableSettingsManager
    {
        abstract static T Pick<T>(string key);
        abstract static T PickNoException<T>(string key);
        abstract static void InitIfNotExist<T>(params (string key, object value)[] keyValue);
        abstract static void Save<T>(params (string key, object value)[] keyValue);
    }
}
