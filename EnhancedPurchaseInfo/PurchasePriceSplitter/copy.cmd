set platform=%1
set configuration=%2
set framework=%3
set currentDir=%CD%
rem echo CurrentDir: %currentDir%
set targetDir=%currentDir%\..\EnhancedPurchaseInfo\bin\x86\%configuration%\%framework%
rem echo TargetDir: %targetDir%
md %targetDir%
md %targetDir%\de
set outDir=%currentDir%\bin\%configuration%\%framework%
rem echo OutDir: %outDir%
copy /y %outDir%\PurchasePriceSplitter.* %targetDir%\
copy /y %outDir%\de\PurchasePriceSplitter.* %targetDir%\de\