//
//  POSTData.h
//  SYNAPSE
//
//  Created by 青井 佑 on 2014/01/15.
//  Copyright (c) 2014年 Quotenter. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface POSTData : NSObject

- (void)postDataTo:(NSURL *)url withKeys:(NSArray *)keys;

@end
