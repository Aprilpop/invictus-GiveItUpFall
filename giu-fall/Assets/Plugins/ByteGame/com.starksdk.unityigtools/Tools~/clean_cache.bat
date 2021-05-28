@echo off

set ADB=%1
set DEVICE_ID=%2
set APP_ID=%3
set CLEAN_TYPE=%4
set APP_HOST=%5

if "%APP_HOST%" == "" (
    echo "Usage: %0 adb_path device_id app_id clean_type app_host(tt, douyin, douyin_lite)"
    exit 1
)

if not exist %ADB% (
    set ADB=adb.exe
)

echo ADB: %ADB%
echo DEVICE_ID: %DEVICE_ID%
echo APP_ID: %APP_ID%
echo APP_HOST: %APP_HOST%
echo CLEAN_TYPE: %CLEAN_TYPE%

if "%APP_HOST%" == "tt" (
    set PACKAGE_NAME=com.ss.android.article.news
) else if "%APP_HOST%" == "tt_lite" (
    set PACKAGE_NAME=com.ss.android.article.lite
) else if "%APP_HOST%" == "douyin" (
    set PACKAGE_NAME=com.ss.android.ugc.aweme
) else if "%APP_HOST%" == "douyin_lite" (
    set PACKAGE_NAME=com.ss.android.ugc.aweme.lite
) else (
    echo Error: invalid app_host: %APP_HOST%
    exit 1
)
%ADB% -s %DEVICE_ID% shell am force-stop %PACKAGE_NAME%
timeout 1 > nul
if "%CLEAN_TYPE%" == "preview" (
    %ADB% -s %DEVICE_ID% shell rm -rf /sdcard/Android/data/%PACKAGE_NAME%/files/_ucfiles/preview
    %ADB% -s %DEVICE_ID% shell rm -rf /sdcard/Android/data/%PACKAGE_NAME%/files/_ucfiles/Unity/preview
) else if "%CLEAN_TYPE%" == "latest" (
    %ADB% -s %DEVICE_ID% shell rm -rf /sdcard/Android/data/%PACKAGE_NAME%/files/_ucfiles/latest
    %ADB% -s %DEVICE_ID% shell rm -rf /sdcard/Android/data/%PACKAGE_NAME%/files/_ucfiles/Unity/latest
) else if "%CLEAN_TYPE%" == "all" (
    %ADB% -s %DEVICE_ID% shell rm -rf /sdcard/Android/data/%PACKAGE_NAME%/files/_ucfiles
) else (
    echo Error: invalid clean_type: %CLEAN_TYPE%
    exit 1
)

%ADB% -s %DEVICE_ID% shell rm -rf /sdcard/Android/data/%PACKAGE_NAME%/files/_ucfiles/local
%ADB% -s %DEVICE_ID% shell rm -rf /sdcard/Android/data/%PACKAGE_NAME%/files/_ucfiles/Unity/local
echo clean cache done.
