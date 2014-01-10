//
//  SHA1Generator.h
//  SYNAPSE
//
//  Created by 青井 佑 on 2014/01/10.
//  Copyright (c) 2014年 Quotenter. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface SHA1Generator : NSObject

+ (NSString *) sha1WithString:(NSString *)input;

@end
