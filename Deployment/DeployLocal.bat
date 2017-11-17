SET MS_BUILD="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
call %MS_BUILD% Deploy.target /target:Deploy /verbosity:m || pause