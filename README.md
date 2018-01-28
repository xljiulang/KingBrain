* 1、启动KingQuestionProxy服务和KingAnswerServer
* 2、给手机wifi设置代理：{KingQuestionProxy所在ip}:5533
* 3、手机浏览器访问{KingQuestionProxy所在ip}:5533，下载<FiddlerRoot certificate>
* 4、设置->高级选项->安全->凭据存储->从SD卡安装，选择下载的FiddlerRoot.cer
* 5、手机访问百度(https的网站)，能看到内容说明成功了，玩一下游戏，KingAnswerServer就自动显示答案
