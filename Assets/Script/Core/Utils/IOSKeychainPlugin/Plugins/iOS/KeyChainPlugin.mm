#import "KeyChainPlugin.h"
#import "UICKeyChainStore.h"
 
@implementation KeyChainPlugin
 
extern "C" {
    char* getKeyChainValue(const char* key);
    void setKeyChainValue(const char* key, const char* value);
    void deleteKeyChainValue(const char* key);
}

char* getKeyChainValue(const char* key)
{
	NSString *keyString = [NSString stringWithCString: key encoding:NSUTF8StringEncoding];
	NSString *value = [UICKeyChainStore stringForKey:keyString];
	if (value == nil || [value isEqualToString:@""])
	{
		value = @"";
	}
	return makeStringCopy([value UTF8String]);
}
void setKeyChainValue(const char* key, const char* value)
{
	NSString *keyString = [NSString stringWithCString: key encoding:NSUTF8StringEncoding];
    NSString *valueString = [NSString stringWithCString: value encoding:NSUTF8StringEncoding]; 
    [UICKeyChainStore setString:valueString forKey:keyString];
} 
void deleteKeyChainValue(const char* key)
{
	NSString *keyString = [NSString stringWithCString: key encoding:NSUTF8StringEncoding];
    [UICKeyChainStore removeItemForKey:keyString];
} 
char* makeStringCopy(const char* str)
{
    if (str == NULL) {
        return NULL;
    } 
    char* res = (char*)malloc(strlen(str) + 1);
    strcpy(res, str);
    return res;
}
 
@end