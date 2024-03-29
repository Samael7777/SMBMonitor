﻿using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using SmbMonitor.NativeApi.NativeErrors.Win32;

namespace SmbMonitor.NativeApi.NativeErrors;

public static class Win32ErrorChecker
{
    /// <summary>
    ///     Проверка кода ошибки с генерацией исключения
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="methodName">Название метода, где происходит проверка</param>
    /// <exception cref="Win32Exception"></exception>
    public static void CheckErrorWithException(int errorCode, string methodName = "")
    {
        if (errorCode == (int)Win32ErrorCode.ERROR_SUCCESS) return;

        var message = CreateExceptionMessage
            (methodName, errorCode);
        throw new Win32Exception(errorCode, message);
    }

    public static void CheckLastErrorWithException(string methodName)
    {
        var lastError = Marshal.GetLastWin32Error();
        if (lastError == (int)Win32ErrorCode.ERROR_SUCCESS) return;

        var errorMessage = CreateExceptionMessage(methodName, lastError);
        throw new Win32Exception($"{methodName} : {errorMessage}");
    }

    private static string CreateExceptionMessage(string methodName, int errorCode)
    {
        //const uint en_US = 0x409;
        const uint ru_RU = 0x419;

        var convertedError = ConvertErrorCode(errorCode);

        var message = new StringBuilder
            ($"MethodName: {methodName}, ErrorCode: 0x{convertedError:X8}");

        var errorDescription = ErrorMessageBuilder.GetErrorDescription
            (convertedError, ru_RU);

        message.Append($", ErrorMessage: {errorDescription}");

        return message.ToString();
    }

    private static uint ConvertErrorCode(int errorCode)
    {
        return errorCode <= 0
            ? (uint)errorCode
            : (uint)((errorCode & 0x0000FFFF) | (7 << 16) | unchecked((int)0x80000000));
    }
}