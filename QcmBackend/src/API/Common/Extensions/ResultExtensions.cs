using System;
using System.Threading.Tasks;
using QcmBackend.Application.Common.Result;

namespace QcmBackend.API.Common.Extensions
{
    public static class ResultExtensions
    {
        public static T Match<T>(
            this Result result,
            Func<T> onSuccess,
            Func<Error, T> onFailure)
        {
            return result.Succeeded ? onSuccess() : onFailure(result.Error!);
        }

        public static T Match<T, TValue>(
            this Result<TValue> result,
            Func<TValue, T> onSuccess,
            Func<Error, T> onFailure)
        {
            return result.Succeeded ? onSuccess(result.Value) : onFailure(result.Error!);
        }

        public static async Task<T> MatchAsync<T>(
            this Result result,
            Func<Task<T>> onSuccess,
            Func<Error, Task<T>> onFailure)
        {
            return result.Succeeded ? await onSuccess() : await onFailure(result.Error!);
        }

        public static async Task<T> MatchAsync<T, TValue>(
            this Result<TValue> result,
            Func<TValue, Task<T>> onSuccess,
            Func<Error, Task<T>> onFailure)
        {
            return result.Succeeded ? await onSuccess(result.Value) : await onFailure(result.Error!);
        }

        public static async Task<T> MatchAsync<T>(
            this Result result,
            Func<Task<T>> onSuccess,
            Func<Error, T> onFailure)
        {
            return result.Succeeded ? await onSuccess() : onFailure(result.Error!);
        }

        public static async Task<T> MatchAsync<T, TValue>(
            this Result<TValue> result,
            Func<TValue, Task<T>> onSuccess,
            Func<Error, T> onFailure)
        {
            return result.Succeeded ? await onSuccess(result.Value) : onFailure(result.Error!);
        }

        public static async Task<T> MatchAsync<T>(
            this Result result,
            Func<T> onSuccess,
            Func<Error, Task<T>> onFailure)
        {
            return result.Succeeded ? onSuccess() : await onFailure(result.Error!);
        }

        public static async Task<T> MatchAsync<T, TValue>(
            this Result<TValue> result,
            Func<TValue, T> onSuccess,
            Func<Error, Task<T>> onFailure)
        {
            return result.Succeeded ? onSuccess(result.Value) : await onFailure(result.Error!);
        }
    }
}