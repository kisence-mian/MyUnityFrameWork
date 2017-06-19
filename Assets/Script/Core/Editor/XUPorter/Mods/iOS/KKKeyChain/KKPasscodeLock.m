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

#import "KKPasscodeLock.h"
#import "KKKeychain.h"
#import "KKPasscodeViewController.h"

static KKPasscodeLock *sharedLock = nil;


@implementation KKPasscodeLock

@synthesize eraseOption = _eraseOption;
@synthesize attemptsAllowed = _attemptsAllowed;

+ (KKPasscodeLock*)sharedLock
{
	@synchronized(self) {
		if (sharedLock == nil) {
			sharedLock = [[self alloc] init];
			sharedLock.eraseOption = YES;
			sharedLock.attemptsAllowed = 5;
		}
	}
	return sharedLock;
}

- (BOOL)isPasscodeRequired
{
	return [[KKKeychain getStringForKey:@"passcode_on"] isEqualToString:@"YES"];
}

- (void)setDefaultSettings
{
	if (![KKKeychain getStringForKey:@"passcode_on"]) {
		[KKKeychain setString:@"NO" forKey:@"passcode_on"];
	}
	
	if (![KKKeychain getStringForKey:@"erase_data_on"]) {
		[KKKeychain setString:@"NO" forKey:@"erase_data_on"];
	}
}


@end
