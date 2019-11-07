using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;

#if UNITY_EDITOR_OSX

using UnityEditor.iOS.Xcode;
using UnityEditor.XCodeEditor;

#endif


public class Package
{


#if UNITY_EDITOR_OSX

        
   [PostProcessBuildAttribute(100)]
   public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject) {
       Debug.LogWarning("IphoneX Fiter");
       if (target != BuildTarget.iOS) {
           Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
           return;
       }
        // Create a new project object from build target
        UnityEditor.iOS.Xcode.PBXProject project = new UnityEditor.iOS.Xcode.PBXProject();
       string configFilePath = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(pathToBuiltProject);
       project.ReadFromFile(configFilePath);
       string targetGuid = project.TargetGuidByName("Unity-iPhone");
       string debug = project.BuildConfigByName(targetGuid, "Debug");
       string release = project.BuildConfigByName(targetGuid, "Release");
       project.AddBuildPropertyForConfig(debug, "CODE_SIGN_RESOURCE_RULES_PATH", "$(SDKROOT)/ResourceRules.plist");
       project.AddBuildPropertyForConfig(release, "CODE_SIGN_RESOURCE_RULES_PATH", "$(SDKROOT)/ResourceRules.plist");

       project.AddFrameworkToProject(targetGuid, "SystemConfiguration.framework", true);
       project.AddFrameworkToProject(targetGuid, "Security.framework", true);
       project.AddFrameworkToProject(targetGuid, "libz.tbd", true);
       project.AddFrameworkToProject(targetGuid, "libc++.tbd", true);

       project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

       project.WriteToFile(configFilePath);

       EditSuitIpXCode(pathToBuiltProject);
        EditSuitIpXCodeHomeButton(pathToBuiltProject);
   }

   public static void EditSuitIpXCode(string path) {
       //插入代码
       //读取UnityAppController.mm文件
       string src = @"_window         = [[UIWindow alloc] initWithFrame: [UIScreen mainScreen].bounds];";
       string dst = @"//    _window         = [[UIWindow alloc] initWithFrame: [UIScreen mainScreen].bounds];

 CGRect winSize = [UIScreen mainScreen].bounds;
    NSString *stringFloat = [NSString stringWithFormat:@""%f"",winSize.size.height];
    NSLog(@""height=>%@"",stringFloat);
    NSString *stringFloat1 = [NSString stringWithFormat:@""%f"",winSize.size.width];
    NSLog(@""width=>%@"",stringFloat1);
    struct utsname systemInfo;
    uname(&systemInfo);
    NSString *deviceString = [NSString stringWithCString:systemInfo.machine encoding:NSUTF8StringEncoding];
    

    BOOL b = NO; // liuhai
    if ([deviceString isEqualToString:@""iPhone10,3""] ||
        [deviceString isEqualToString:@""iPhone10,6""]) b = YES; //iPhone X
    if ([deviceString isEqualToString:@""iPhone11,8""]) b = YES; //iPhone XR
    if ([deviceString isEqualToString:@""iPhone11,2""]) b = YES; //iPhone XS
    if ([deviceString isEqualToString:@""iPhone11,4""] ||
        [deviceString isEqualToString:@""iPhone11,6""]) b = YES; //iPhone XS Max
    if ([deviceString isEqualToString:@""x86_64""])  b = YES; //iPhoneSimulator

    NSLog(@""deviceString=>%@"",deviceString);
    if(b)
    {
        if(winSize.size.height> winSize.size.width)
        {
            winSize.size.height -= 78;
            winSize.origin.y = 34;
            ::printf(""-> is iphonex aaa hello world\n"");
        }
        else
        {
            winSize.size.width -= 78;
            winSize.origin.x = 34;
            ::printf(""-> is not iphonex aaa hello world\n"");
        }
        
    }else{
         ::printf(""-> b is no\n"");
    }
 _window = [[UIWindow alloc] initWithFrame: winSize];
   ";

       string unityAppControllerPath = path + "/Classes/UnityAppController.mm";
       XClassExt UnityAppController = new XClassExt(unityAppControllerPath);
       UnityAppController.Replace(src, dst);

        src = @"#include <sys/sysctl.h>";
        dst = @"#include <sys/sysctl.h> 
    "+
        @"#include <sys/utsname.h>";
        
        UnityAppController.Replace(src, dst);
    }
    //修改锁定手势，苹果Xhome建
    public static void EditSuitIpXCodeHomeButton(string path)
    {
        //插入代码
        //读取UnityViewControllerBase+iOS.mm文件
        string src = @"- (UIRectEdge)preferredScreenEdgesDeferringSystemGestures";
        string dst = @"- (UIRectEdge)preferredScreenEdgesDeferringSystemGestures
{
    //UIRectEdge res = UIRectEdgeNone;
  //  if (UnityGetDeferSystemGesturesTopEdge())
   //     res |= UIRectEdgeTop;
    //if (UnityGetDeferSystemGesturesBottomEdge())
   //     res |= UIRectEdgeBottom;
   //if (UnityGetDeferSystemGesturesLeftEdge())
   //    res |= UIRectEdgeLeft;
  // if (UnityGetDeferSystemGesturesRightEdge())
    //   res |= UIRectEdgeRight;
   // return res;
    return UIRectEdgeAll;
    
}
- (UIRectEdge)preferredScreenEdgesDeferringSystemGestures1";

        string unityAppControllerPath = path + "/Classes/UI/UnityViewControllerBase+iOS.mm";
        XClassExt UnityAppController = new XClassExt(unityAppControllerPath);
        UnityAppController.Replace(src, dst);

        src = @"- (BOOL)prefersHomeIndicatorAutoHidden";
        dst = @"- (BOOL)prefersHomeIndicatorAutoHidden
{
    return NO;
}
- (BOOL)prefersHomeIndicatorAutoHidden1;";

        UnityAppController.Replace(src, dst);
    }

#endif

}
