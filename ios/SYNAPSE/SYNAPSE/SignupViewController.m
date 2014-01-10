//
//  SignupViewController.m
//  SYNAPSE
//
//  Created by 青井 佑 on 2014/01/08.
//  Copyright (c) 2014年 Quotenter. All rights reserved.
//

#import "SignupViewController.h"

@interface SignupViewController ()

@end

@implementation SignupViewController

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
	// Do any additional setup after loading the view.
    _navBar.delegate = self;
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (IBAction)pushedSignupButton:(id)sender {
    NSURL *url = [NSURL URLWithString:@"http://synapse-server.cloudapp.net/SignUp.aspx"];
    NSMutableURLRequest *request = [[NSMutableURLRequest alloc] initWithURL:url];
    NSString *keyValue = [NSString stringWithFormat:@"uid=%@&nn=%@&mail=%@&ph=%@&did=999999", self.userIDField.text, self.nicknameField.text, self.mailAddressField.text, self.passwordField1.text];
    NSData *post = [keyValue dataUsingEncoding:NSUTF8StringEncoding allowLossyConversion:YES];
    
    [request setHTTPMethod:@"POST"];
    [request setHTTPBody:post];
    
    // Post Data
    NSData *response = [NSURLConnection sendSynchronousRequest:request returningResponse:nil error:nil];
}

- (UIBarPosition)positionForBar:(id <UIBarPositioning>)bar
{
    return UIBarPositionTopAttached;
}

- (IBAction)toggleAgreeLicense:(id)sender {
    if ([_signupButton isEnabled] == NO) {
        [_signupButton setEnabled:YES];
    }else{
        [_signupButton setEnabled:NO];
    }
}

- (IBAction)closeKeyboard:(id)sender {
    [sender resignFirstResponder];
}

- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event {
    
    UITouch *touch = [[event allTouches] anyObject];
    if ([_passwordField1 isFirstResponder] && [touch view] != _passwordField1) {
        [_passwordField1 resignFirstResponder];
    }
    [super touchesBegan:touches withEvent:event];
}

@end
