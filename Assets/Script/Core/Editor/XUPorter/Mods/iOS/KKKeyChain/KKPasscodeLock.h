//
// Copyright 2011-2012 Kosher Penguin LLC 
// Created by Adar Porat (https://github.com/aporat) on 1/16/2012.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//		http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#import <Foundation/Foundation.h>
#import "KKPasscodeViewController.h"

@interface KKPasscodeLock : NSObject {
  
  // whatever the erase option is enabled in the passcode settings
	BOOL _eraseOption;
  
  // how many attemepts is user is allowed to have before the screen is locked
	NSUInteger _attemptsAllowed;
}

/**
 * a shared object which can change the passcode settings and perform generic actions
 */
+ (KKPasscodeLock*)sharedLock;

/**
 * checks if a passcode has to be displayed
 */
- (BOOL)isPasscodeRequired;


/**
 * set the initial settings of the passcode settings
 */
- (void)setDefaultSettings;

@property (nonatomic,assign) BOOL eraseOption;

@property (nonatomic,assign) NSUInteger attemptsAllowed;

@end
