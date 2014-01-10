//
//  ModelController.h
//  SYNAPSE
//
//  Created by 青井 佑 on 2013/12/23.
//  Copyright (c) 2013年 Quotenter. All rights reserved.
//

#import <UIKit/UIKit.h>

@class DataViewController;

@interface ModelController : NSObject <UIPageViewControllerDataSource>

- (DataViewController *)viewControllerAtIndex:(NSUInteger)index storyboard:(UIStoryboard *)storyboard;
- (NSUInteger)indexOfViewController:(DataViewController *)viewController;

@end
