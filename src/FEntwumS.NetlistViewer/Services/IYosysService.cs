﻿
using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Services;

public interface IYosysService
{
    Task<bool> LoadVhdlAsync(IProjectFile file);
    Task<bool> LoadVerilogAsync(IProjectFile file);
    Task<bool> LoadSystemVerilogAsync(IProjectFile file);
    Task<bool> CreateJsonNetlistAsync();
    Task<bool> CreateVerilogAsync();
}