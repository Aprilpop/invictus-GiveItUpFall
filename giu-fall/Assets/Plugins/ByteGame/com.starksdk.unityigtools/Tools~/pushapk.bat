@echo off

set ADB=%1
set DEVICE_ID=%2
set APK_PATH=%3
set APP_HOST=%4

if "%APP_HOST%" == "" (
    echo "Usage: %0 adb_path device_id apk_path app_host(tt, douyin, douyin_lite)"
    exit 1
)

if not exist %ADB% (
    set ADB=adb.exe
)

if not exist %APK_PATH% (
    echo Error: %APK_PATH% not exists
    exit 1
)

echo ADB: %ADB%
echo DEVICE_ID: %DEVICE_ID%
echo APK_PATH: %APK_PATH%
echo APP_HOST: %APP_HOST%

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

set ANDROID_LOCAL_PATH=/sdcard/Android/data/%PACKAGE_NAME%/files/_ucfiles/local/main.apk
echo push %APK_PATH% to %ANDROID_LOCAL_PATH%
%ADB% -s %DEVICE_ID% push %APK_PATH% %ANDROID_LOCAL_PATH%
