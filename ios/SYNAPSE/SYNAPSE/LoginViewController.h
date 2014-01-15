//
//  LoginViewController.h
//  SYNAPSE
//
//  Created by 青井 佑 on 2014/01/08.
//  Copyright (c) 2014年 Quotenter. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface LoginViewController : UIViewController

@property (weak, nonatomic) IBOutlet UIButton *loginButton;
@property (weak, nonatomic) IBOutlet UITextField *userIDField;
@property (weak, nonatomic) IBOutlet UITextField *mailAddressField;
@property (weak, nonatomic) IBOutlet UITextField *passwordField;
@property (weak, nonatomic) IBOutlet UINavigationBar *navBar;

- (IBAction)pushedLoginButton:(id)sender;

@end
