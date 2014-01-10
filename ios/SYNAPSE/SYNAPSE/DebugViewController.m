//
//  DebugViewController.m
//  SYNAPSE
//
//  Created by 青井 佑 on 2014/01/10.
//  Copyright (c) 2014年 Quotenter. All rights reserved.
//

#import "DebugViewController.h"
#import "SHA1Generator.h"

@interface DebugViewController ()

@end

@implementation DebugViewController

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
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (IBAction)pushedSHA1Button:(id)sender {
    NSUUID *uuid = [[UIDevice currentDevice] identifierForVendor];
    self.sha1Field.text = [uuid UUIDString];
    self.sha1Label.text = [SHA1Generator sha1WithString:self.sha1Field.text];
}
@end
