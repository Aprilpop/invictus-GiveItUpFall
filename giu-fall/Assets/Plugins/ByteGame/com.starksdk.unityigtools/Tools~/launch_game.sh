if [ $# -lt 4 ]; then
    echo "Usage: $0 adb_path device_id app_id runtime_env app_host(tt, douyin, douyin_lite)"
    exit 1
fi

ADB=$1
DEVICE_ID=$2
APP_ID=$3
RUNTIME_ENV=$4
APP_HOST=$5

if [ ! -f $ADB ]; then
    ADB = "adb"
fi

echo "ADB:" $ADB
echo "DEVICE_ID:" $DEVICE_ID
echo "APP_ID:" $APP_ID
echo "APP_HOST:" $APP_HOST
echo "RUNTIME_ENV:" $RUNTIME_ENV

if [ ! command -v $ADB &> /dev/null ]; then
    echo "Error: adb is not installed! \$ADB: $ADB" >&2
    exit 1
fi

if [ "$APP_HOST" = "tt" ]; then
    echo "launch toutiao uc game"
    $ADB -s $DEVICE_ID shell am force-stop com.ss.android.article.news
    sleep 1
    $ADB -s $DEVICE_ID shell am start -n com.ss.android.article.news/com.ss.android.article.news.activity.SplashActivity
    sleep 1
    $ADB -s $DEVICE_ID shell am start -a android.intent.action.VIEW -d "snssdk143://microgame?version=v2'&'app_id=${APP_ID}'&'scene=011004'&'version_type=${RUNTIME_ENV}'&'bdp_log=%7B%22launch_from%22%3A%22stark_sdk_tools%22%7D'&'tech_type=7'&'bdpsum=5a29d84"
elif [ "$APP_HOST" = "tt_lite" ]; then
    echo "launch toutiao lite uc game"
    $ADB -s $DEVICE_ID shell am force-stop com.ss.android.article.lite
    sleep 1
    $ADB -s $DEVICE_ID shell am start -n com.ss.android.article.lite/com.ss.android.article.lite.activity.SplashActivity
    sleep 1
    $ADB -s $DEVICE_ID shell am start -a android.intent.action.VIEW -d "snssdk35://microgame?version=v2'&'app_id=${APP_ID}'&'scene=011004'&'version_type=${RUNTIME_ENV}'&'bdp_log=%7B%22launch_from%22%3A%22stark_sdk_tools%22%7D'&'tech_type=7'&'bdpsum=5a29d84"
elif [ "$APP_HOST" = "douyin" ]; then
    echo "launch douyin uc game"
    $ADB -s $DEVICE_ID shell am force-stop com.ss.android.ugc.aweme
    sleep 1
    $ADB -s $DEVICE_ID shell am start -n com.ss.android.ugc.aweme/com.ss.android.ugc.aweme.splash.SplashActivity
    sleep 1
    $ADB -s $DEVICE_ID shell am start -a android.intent.action.VIEW -d "snssdk1128://microgame?app_id=${APP_ID}'&'version=v2'&'scene=011004'&'version_type=${RUNTIME_ENV}'&'bdp_log=%7B%22launch_from%22%3A%22stark_sdk_tools%22%7D'&'tech_type=7'&'bdpsum=b27744e"
elif [ "$APP_HOST" = "douyin_lite" ]; then
    echo "launch douyin lite uc game"
    $ADB -s $DEVICE_ID shell am force-stop com.ss.android.ugc.aweme.lite
    sleep 1
    $ADB -s $DEVICE_ID shell am start -n com.ss.android.ugc.aweme.lite/com.ss.android.ugc.aweme.splash.SplashActivity
    sleep 1
    $ADB -s $DEVICE_ID shell am start -a android.intent.action.VIEW -d "snssdk2329://microgame?app_id=${APP_ID}'&'version=v2'&'scene=011004'&'version_type=${RUNTIME_ENV}'&'bdp_log=%7B%22launch_from%22%3A%22stark_sdk_tools%22%7D'&'tech_type=7'&'bdpsum=b27744e"
else
    echo "Error: invalid app_host:" $APP_HOST
    exit 1
fi
echo "launch game completed"
