namespace QcmBackend.Application.Common.Include
{
    public interface IIncludeConfigurator<T>
    {
        IncludableProperties<T> Configure();
    }
}
