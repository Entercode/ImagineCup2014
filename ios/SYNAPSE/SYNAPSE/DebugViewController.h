//
//  DebugViewController.h
//  SYNAPSE
//
//  Created by 青井 佑 on 2014/01/10.
//  Copyright (c) 2014年 Quotenter. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface DebugViewController : UIViewController

@property (weak, nonatomic) IBOutlet UITextField *sha1Field;
@property (weak, nonatomic) IBOutlet UILabel *sha1Label;

- (IBAction)pushedSHA1Button:(id)sender;


@end
