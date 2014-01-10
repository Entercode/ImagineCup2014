//
//  SHAGenerator.m
//  SYNAPSE
//
//  Created by 青井 佑 on 2014/01/10.
//  Copyright (c) 2014年 Quotenter. All rights reserved.
//

#import "SHA1Generator.h"
#include <CommonCrypto/CommonDigest.h>

@implementation SHA1Generator

+ (NSString *) sha1WithString:(NSString *)input
{
    NSData *data = [input dataUsingEncoding:NSUTF8StringEncoding];
    
    uint8_t digest[CC_SHA1_DIGEST_LENGTH];
    
    CC_SHA1(data.bytes, data.length, digest);
    
    NSMutableString *output = [NSMutableString stringWithCapacity:CC_SHA1_DIGEST_LENGTH * 2];
    
    for(int i = 0; i < CC_SHA1_DIGEST_LENGTH; i++)
        [output appendFormat:@"%02x", digest[i]];
    
    return output;
}

@end
