//
//  BluetoothLEManager.m
//  SYNAPSE
//
//  Created by 青井 佑 on 2014/01/10.
//  Copyright (c) 2014年 Quotenter. All rights reserved.
//

#import "BluetoothLEManager.h"

@implementation BluetoothLEManager

- (id)init
{
    self = [super init];
    if (self) {
        /// BLE 通信全般の処理が実行される queue を第 2 引数としてわたす。nil を渡した場合は mainQueue で実行される。
        //_centralManagerSerialGCDQueue = dispatch_queue_create("jp.co.fenrir.BLESample.centralmanager", DISPATCH_QUEUE_SERIAL);
        //self.centralManager = [[CBCentralManager alloc] initWithDelegate:self queue:_centralManagerSerialGCDQueue];
        self.centralManager = [[CBCentralManager alloc] initWithDelegate:self queue:nil];
        
        /// 接続するペリフェラルを保持するためのセットを生成
        self.peripherals = [NSMutableSet set];
    }
    return self;
}

- (void)searchPeripherals {
    /// Scan 時に任意の Service を持っている機器だけを探すように指定できるが SensorTag は Advertise に Service 情報を載せてくれていないため、ここでは nil を指定。
    [self.centralManager scanForPeripheralsWithServices:nil options:options];
}

- (void)centralManager:(CBCentralManager *)central didDiscoverPeripheral:(CBPeripheral *)peripheral advertisementData:(NSDictionary *)advertisementData RSSI:(NSNumber *)RSSI
{
    NSString *localName = [advertisementData objectForKey:CBAdvertisementDataLocalNameKey];
    if ([localName length] && [localName rangeOfString:@"SensorTag"].location != NSNotFound) {
        
        [self updateLabel:self.connectionStatusLabel text:@"接続開始"];
        
        /// Scan を停止させる
        [self.centralManager stopScan];
        
        /// CBPeripheral のインスタンスを保持しなければならない
        [self.peripherals addObject:peripheral];
        
        [self.centralManager connectPeripheral:peripheral options:nil];
    }
}

- (void)centralManager:(CBCentralManager *)central didConnectPeripheral:(CBPeripheral *)peripheral
{
    [self updateLabel:self.connectionStatusLabel text:@"Service検索中"];
    
    /// Peripheral にデリゲートをセットし、Service を探す。今回はボタン押下のサービスのみを探す
    peripheral.delegate = self;
    [peripheral discoverServices:@[[CBUUID UUIDWithString:@"ffe0"]]];
}

- (void)peripheral:(CBPeripheral *)peripheral didDiscoverServices:(NSError *)error
{
    [self updateLabel:self.connectionStatusLabel text:@"Characteristic検索中"];
    
    /// Characteristic を全て探す
    for (CBService * service in peripheral.services) {
        [peripheral discoverCharacteristics:nil forService:service];
    }
}

- (void)peripheral:(CBPeripheral *)peripheral didDiscoverCharacteristicsForService:(CBService *)service error:(NSError *)error
{
    [self updateLabel:self.connectionStatusLabel text:@"Notifyセット"];
    
    /// Characteristic に対して Notify を受け取れるようにする
    for (CBService *service in peripheral.services) {
        for (CBCharacteristic *characteristic in service.characteristics) {
            [peripheral setNotifyValue:YES forCharacteristic:characteristic];
        }
    }
}

- (void)peripheral:(CBPeripheral *)peripheral didUpdateValueForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    /**
     Sensor Tag の場合 ボタンから
     * <00>
     * ボタンが離された
     * <01>
     * 右ボタンが押された
     * <02>
     * 左ボタンが押された
     のいずれかの NSData が返ってくる
     */
    
    UInt8 keyPress = 0;
    [characteristic.value getBytes:&keyPress length:1];
    
    NSString *text = @"押されていない";
    if (keyPress == 1) {
        text = @"右";
    } else if (keyPress == 2) {
        text = @"左";
    } else if (keyPress == 3) {
        text = @"両方";
    }
    
    /// BLE の通信がバックグラウンドスレッドメインスレッドでラベルの更新
    [self updateLabel:self.pressedButtonLabel text:text];
}

@end
