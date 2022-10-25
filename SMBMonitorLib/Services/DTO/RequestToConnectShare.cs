using Base;
using SmbAPI.Base;

namespace SmbMonitorLib.Services.DTO;

internal record RequestToConnectShare(SmbPath share, char diskLetter, Credentials Credentials);