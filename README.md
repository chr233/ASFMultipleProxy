# ASF Multiple Proxy

[![Bilibili](https://img.shields.io/badge/bilibili-Chr__-00A2D8.svg?logo=bilibili)](https://space.bilibili.com/5805394)
[![Steam](https://img.shields.io/badge/steam-Chr__-1B2838.svg?logo=steam)](https://steamcommunity.com/id/Chr_)

[![Steam](https://img.shields.io/badge/steam-donate-1B2838.svg?logo=steam)](https://steamcommunity.com/tradeoffer/new/?partner=221260487&token=xgqMgL-i)
[![爱发电][afdian_img]][afdian_link]
[![buy me a coffee][bmac_img]][bmac_link]

> 用于让 ASF 同时使用多个代理地址, 适用于机器人账号数量特别多的情况
> 指定全局代理池后每个机器人将会随机使用一个代理
> 也可以为机器人独立设置代理

## EULA

> 请不要使用本插件来进行不受欢迎的行为, 包括但不限于: 刷好评, 发布广告 等.
>
> 详见 [插件配置说明](#插件配置说明)

## 安装方式

### 初次安装 / 手动更新

1. 从 [GitHub Releases](https://github.com/chr233/ASFMultipleProxy/releases) 下载插件的最新版本
2. 解压后将 `ASFMultipleProxy.dll` 丢进 `ArchiSteamFarm` 目录下的 `plugins` 文件夹
3. 重新启动 `ArchiSteamFarm` , 使用命令 `ASFMultipleProxy` 或 `AMP` 来检查插件是否正常工作

### ASFEnhance 联动

> 推荐搭配 [ASFEnhance](https://github.com/chr233/ASFEnhance) 使用, 可以通过 ASFEnhance 实现插件更新管理和禁用特定命令等功能

### 捐赠

|               ![img][afdian_qr]                |                   ![img][bmac_qr]                   |                       ![img][usdt_qr]                       |
| :--------------------------------------------: | :-------------------------------------------------: | :---------------------------------------------------------: |
| ![爱发电][afdian_img] <br> [链接][afdian_link] | ![buy me a coffee][bmac_img] <br> [链接][bmac_link] | ![USDT][usdt_img] <br> `TW41eecZ199QK6zujgKP4j1cz2bXzRus3c` |

[afdian_qr]: https://raw.chrxw.com/chr233/master/afadian_qr.png
[afdian_img]: https://img.shields.io/badge/爱发电-@chr__-ea4aaa.svg?logo=github-sponsors
[afdian_link]: https://afdian.net/@chr233
[bmac_qr]: https://raw.chrxw.com/chr233/master/bmc_qr.png
[bmac_img]: https://img.shields.io/badge/buy%20me%20a%20coffee-@chr233-yellow?logo=buymeacoffee
[bmac_link]: https://www.buymeacoffee.com/chr233
[usdt_qr]: https://raw.chrxw.com/chr233/master/usdt_qr.png
[usdt_img]: https://img.shields.io/badge/USDT-TRC20-2354e6.svg?logo=bitcoin

## 插件配置说明

### 全局配置

ASF.json

```json
{
  //ASF 配置
  "CurrentCulture": "...",
  "IPCPassword": "...",
  "...": "...",
  //Asf Award Tool 配置
  "ASFEnhance": {
    "EULA": true,
    "Statistic": true,
    "MultWebProxy": [
      {
        "WebProxy": "http://example1.com",
        "WebProxyUsername": "",
        "WebProxyPassword": ""
      },
      {
        "WebProxy": "http://example2.com",
        "WebProxyUsername": "",
        "WebProxyPassword": ""
      },
      {
        "WebProxy": "http://example3.com",
        "WebProxyUsername": "",
        "WebProxyPassword": ""
      }
    ],
    "IpInfoToken": "Your Token Here"
  }
}
```

| 配置项                       | 类型              | 默认值 | 说明                                                                           |
| ---------------------------- | ----------------- | ------ | ------------------------------------------------------------------------------ |
| `EULA`                       | `bool`            | `true` | 是否同意 [EULA](#eula)                                                         |
| `Statistic`                  | `bool`            | `true` | 是否允许发送统计数据, 仅用于统计插件用户数量, 不会发送任何其他信息             |
| `MultWebProxy`               | `List<ProxyData>` | `null` | 全局代理列表, 所有机器人会设置为列表中随机代理                                 |
| -                            | -                 | -      | -                                                                              |
| `ProxyData.WebProxy`         | `str`             | `null` | 代理地址                                                                       |
| `ProxyData.WebProxyUsername` | `str`             | `null` | 代理用户名                                                                     |
| `ProxyData.WebProxyPassword` | `str`             | `null` | 代理密码                                                                       |
| -                            | -                 | -      | -                                                                              |
| `IpInfoToken`                | `str`             | `null` | 用于获取当前 IP 信息, 前往 [IpInfo.io](https://ipinfo.io/dashboard/token) 获取 |

### 机器人配置

> 为机器人设置代理将会覆盖全局设置

Bot.json

```json
{
  //Bot 配置
  "BotBehaviour": 40,
  "Enabled": true,
  "...": "...",
  //代理设置
  "WebProxy": "http://10.10.0.15:1083",
  "WebProxyUsername": "",
  "WebProxyPassword": ""
}
```

| 配置项             | 类型     | 默认值 | 说明             |
| ------------------ | -------- | ------ | ---------------- |
| `WebProxy`         | `string` | `null` | 代理地址, 可选   |
| `WebProxyUsername` | `string` | `null` | 代理用户名, 可选 |
| `WebProxyPassword` | `string` | `null` | 代理密码, 可选   |

## 插件指令说明

> `[]` 代表可省略的参数

| 命令               | 缩写  | 权限            | 说明                           |
| ------------------ | ----- | --------------- | ------------------------------ |
| `ASFMultipleProxy` | `AAT` | `FamilySharing` | 查看 ASF Multiple Proxy 的版本 |
| `GETPROXY [Bots]`  |       | `Master`        | 查看当前机器人使用的代理地址   |
| `ASFIP`            |       | `Master`        | 查看 ASF 全局请求的 IP 信息    |
| `BOTIP [Bots]`     |       | `Master`        | 查看指定机器人的 IP 信息       |
