using Base;
using SmbAPI.Base;

namespace SmbMonitorLib.Services.DTO;

internal record ShareConnectRequest(SmbPath Share, char DiskLetter, Credentials Credentials);