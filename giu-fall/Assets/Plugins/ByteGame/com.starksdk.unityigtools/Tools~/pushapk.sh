if [ $# -lt 3 ]; then
    echo "Usage: $0 adb_path device_id apk_path app_host(tt, douyin, douyin_lite)"
    exit 1
fi

ADB=$1
DEVICE_ID=$2
APK_PATH=$3
APP_HOST=$4

if [ ! -f $ADB ]; then
    ADB = "adb"
fi

echo "ADB:" $ADB
echo "DEVICE_ID:" $DEVICE_ID
echo "APK_PATH:" $APK_PATH
echo "APP_HOST:" $APP_HOST

if [ ! command -v $ADB &> /dev/null ]; then
    echo "Error: adb is not installed! ADB: $ADB" >&2
    exit 1
fi

if [ ! -f $APK_PATH ]; then
    echo "Error: $APK_PATH not exists" >&2
    exit 1
fi

if [ "$APP_HOST" = "tt" ]; then
    PACKAGE_NAME=com.ss.android.article.news
elif [ "$APP_HOST" = "tt_lite" ]; then
    PACKAGE_NAME=com.ss.android.article.lite
elif [ "$APP_HOST" = "douyin" ]; then
    PACKAGE_NAME=com.ss.android.ugc.aweme
elif [ "$APP_HOST" = "douyin_lite" ]; then
    PACKAGE_NAME=com.ss.android.ugc.aweme.lite
else
    echo "Error: invalid app_host:" $APP_HOST
    exit 1
fi


ANDROID_LOCAL_PATH=/sdcard/Android/data/${PACKAGE_NAME}/files/_ucfiles/local/main.apk
echo "push ${APK_PATH} to ${ANDROID_LOCAL_PATH}"
$ADB -s $DEVICE_ID push "${APK_PATH}" ${ANDROID_LOCAL_PATH}
