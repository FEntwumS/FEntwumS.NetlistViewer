@echo off
REM YOSYSHQ_ROOT is set by oss-cad-suite\environment.bat
REM script is from oss-cad-suite-build#142
set VERILATOR_ROOT=%YOSYSHQ_ROOT%\share\verilator
verilator_bin %*