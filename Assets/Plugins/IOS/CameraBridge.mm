//
//  CameraBridge.m
//  UnityFramework
//
//  Created by slience on 2023/6/19.
//
#import <UIKit/UIKit.h>
#import <AVKit/AVKit.h>
#import <Photos/Photos.h>
#import <UnityFramework/UnityFramework.h>
#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
@interface CameraBridge : NSObject <UINavigationControllerDelegate, UIImagePickerControllerDelegate>
+(instancetype)shared;
-(void)openCamera;
@end
@implementation CameraBridge

+(instancetype)shared {
    static CameraBridge * camera = nil;
    static dispatch_once_t once;
    dispatch_once(&once, ^{
        camera = [[CameraBridge alloc]init];
    });
    return camera;
}
- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<UIImagePickerControllerInfoKey, id> *)info {
    
    [picker dismissViewControllerAnimated:YES completion:^{
        UIImage * image = [[UIImage alloc]init];
 //       __block PHAsset *phAsset = [[PHAsset alloc]init];
        image = [info valueForKey:UIImagePickerControllerOriginalImage];
        UIImageOrientation imageOrientation = image.imageOrientation;
                if(imageOrientation != UIImageOrientationUp)
                {
                    CGFloat aspectRatio = MIN ( 1920 / image.size.width, 1920 / image.size.height );
                    CGFloat aspectWidth = image.size.width * aspectRatio;
                    CGFloat aspectHeight = image.size.height * aspectRatio;

                    UIGraphicsBeginImageContext(CGSizeMake(aspectWidth, aspectHeight));
                    [image drawInRect:CGRectMake(0, 0, aspectWidth, aspectHeight)];
                    image = UIGraphicsGetImageFromCurrentImageContext();
                    UIGraphicsEndImageContext();
                }
        
        image = [self croppedImage:image WithFrame:CGRectMake(0, 0, image.size.width, image.size.height) inSize:CGSizeMake(350, 350)];
        NSData * data  =   UIImageJPEGRepresentation(image, 1.0);
     
       NSString * string = [data base64EncodedStringWithOptions:NSDataBase64Encoding64CharacterLineLength];
     
     char * cstring = (char *)  [string UTF8String];
      
     
     [[UnityFramework getInstance] sendMessageToGOWithName:"CrossPlatform" functionName:"NativeCallUnity" message:makeStringCopy(cstring)];
    }];
       

    
//       phAsset = [self getPHAssetWithInfo:info LocalIdentifier:nil];
//       if (phAsset == nil) {
//           [self saveImage_Obj:image completion:^(BOOL success, NSError * _Nullable error, NSString * _Nullable localIdentifier) {
//               if (success) {
//                   phAsset = [self getPHAssetWithInfo:info LocalIdentifier:localIdentifier];
//               }
//               if (!success || phAsset == nil) {
//
//                   //
//                   return;
//               }
//               [self getImageData:phAsset completion:^(NSData *imageData) {
//
//               }];
//           }];
//       }
//       else
//       {
//           [self getImageData:phAsset completion:^(NSData *imageData) {
//
//           }];
//       }

}

- (UIImage *)croppedImage:(UIImage *)image WithFrame:(CGRect)frame inSize:(CGSize)inSize
{
  if (inSize.width==0 || inSize.height==0) {
    return nil;
  }
  UIImage *croppedImage = nil;
  CGFloat scaleX=inSize.width/frame.size.width;
  CGFloat scaleY=inSize.height/frame.size.height;
  UIGraphicsBeginImageContextWithOptions(inSize, ![self hasAlpha:image], image.scale);
  {
    CGContextRef context = UIGraphicsGetCurrentContext();
    CGContextTranslateCTM(context, -frame.origin.x*scaleX, -frame.origin.y*scaleY);
    CGContextScaleCTM(context, scaleX, scaleY);
    [image drawAtPoint:CGPointZero];
    
    croppedImage = UIGraphicsGetImageFromCurrentImageContext();
  }
  UIGraphicsEndImageContext();
  //NSLog(@"cropped image size:%@",NSStringFromCGSize(croppedImage.size));
  return [UIImage imageWithCGImage:croppedImage.CGImage scale:image.scale orientation:UIImageOrientationUp];
}

- (BOOL)hasAlpha:(UIImage *)image
{
    CGImageAlphaInfo alphaInfo = CGImageGetAlphaInfo(image.CGImage);
    return (alphaInfo == kCGImageAlphaFirst || alphaInfo == kCGImageAlphaLast ||
            alphaInfo == kCGImageAlphaPremultipliedFirst || alphaInfo == kCGImageAlphaPremultipliedLast);
}
char * makeStringCopy(const char* string)
{
  if (NULL == string) {
    return NULL;
  }
  char* res = (char*)malloc(strlen(string)+1);
  strcpy(res, string);
  return res;
}
-(void)openCamera {
    
    UIImagePickerController *imagePicker = [[UIImagePickerController alloc] init];
    imagePicker.delegate = self;
    imagePicker.sourceType = UIImagePickerControllerSourceTypeCamera;
//    imagePicker.mediaTypes = @[(NSString *)kUTTypeImage];
    imagePicker.modalPresentationStyle = UIModalPresentationOverFullScreen;
    [[self getCurrentVC] presentViewController:imagePicker animated:YES completion:nil];
}
- (UIViewController *)getCurrentVC
{
    //获取当前的UIViewController
    UIViewController*rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;
    
    UIViewController *currentVC = [self getCurrentVCFrom:rootViewController];
    
    return currentVC;
}
- (UIViewController *)getCurrentVCFrom:(UIViewController *)rootVC {
    UIViewController *currentVC;
    
    if ([rootVC presentedViewController]) {
      
        
        rootVC = [rootVC presentedViewController];
    }
    
    if ([rootVC isKindOfClass:[UITabBarController class]]) {

        
        currentVC = [self getCurrentVCFrom:[(UITabBarController *)rootVC selectedViewController]];
        
    } else if ([rootVC isKindOfClass:[UINavigationController class]]){
     
        
        currentVC = [self getCurrentVCFrom:[(UINavigationController *)rootVC visibleViewController]];
        
    } else {
       
     
        currentVC = rootVC;
    }
    
    return currentVC;
}

#pragma mark - 检查相机权限
-(void)checkAVAuthorizationStatusWithAllowToUse:(nullable void(^)(void))allowToUse
{
    //判断是否有相机权限
    AVAuthorizationStatus authorizationStatus = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];//读取设备授权状态
    if (authorizationStatus == AVAuthorizationStatusAuthorized && allowToUse != nil) {
        allowToUse();
    }
    else if(authorizationStatus == AVAuthorizationStatusNotDetermined){
        [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {
            if (granted == YES && allowToUse != nil) {
                allowToUse();
            }
        }];
    } else {
        UIAlertController * alertController = [UIAlertController alertControllerWithTitle:@"提示" message:@"请到设置->隐私->相机中开启【华世界商圈】的使用权限，以便于我们能够使用您的相机进行拍摄或扫描。" preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction * OK_Action = [UIAlertAction actionWithTitle:@"设置" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
            if (@available(iOS 10.0, *)) {
                [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString] options:@{} completionHandler:^(BOOL success)
                 {
                     
                 }];
            } else {
                //iOS10以前,使用旧API
                [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
            }
        }];
        [alertController addAction:OK_Action];
        
        UIAlertAction * Cancel_Action = [UIAlertAction actionWithTitle:@"取消" style:UIAlertActionStyleCancel handler:^(UIAlertAction * _Nonnull action) {
            [[self getCurrentVC] dismissViewControllerAnimated:YES completion:nil];
        }];
        [alertController addAction:Cancel_Action];
        [[self getCurrentVC] presentViewController:alertController animated:YES completion:nil];
    }
}
/*
#pragma mark - 获取PHAsset
-(PHAsset *)getPHAssetWithInfo:(NSDictionary<UIImagePickerControllerInfoKey,id> *)info LocalIdentifier:(NSString *)localIdentifier
{
    PHAsset *phAsset = [[PHAsset alloc]init];
    if (@available(iOS 11.0, *)) {
        phAsset = [info valueForKey:UIImagePickerControllerPHAsset];
    } else {
        // Fallback on earlier versions
        NSURL * imageURL = [info valueForKey:UIImagePickerControllerMediaURL];
        PHFetchResult *fetchResult = [PHAsset fetchAssetsWithALAssetURLs:@[imageURL] options:nil];
        phAsset = fetchResult.firstObject;
        
    }
    return phAsset;
}

#pragma mark - 保存图片方法
-(void)saveImage_Obj:(id)image_Obj
       completion:(nullable void(^)(BOOL success, NSError *__nullable error, NSString *__nullable localIdentifier))completion
{
    // 1. 获取相片库对象
    PHPhotoLibrary *library = [PHPhotoLibrary sharedPhotoLibrary];
    //占位对象
    __block PHObjectPlaceholder * placeholder;
    
    // 2. 调用changeBlock
    [library performChanges:^{
        
        // 2.1 创建一个相册变动请求
        PHAssetCollectionChangeRequest *collectionRequest;
        
        // 2.2 取出指定名称的相册
        PHAssetCollection *assetCollection = [self getCurrentPhotoCollectionWithTitle:collectionName];
        
        // 2.3 判断相册是否存在
        if (assetCollection) {
            // 如果存在就使用当前的相册创建相册请求
            collectionRequest = [PHAssetCollectionChangeRequest changeRequestForAssetCollection:assetCollection];
        } else {
            // 如果不存在, 就创建一个新的相册请求
            collectionRequest = [PHAssetCollectionChangeRequest creationRequestForAssetCollectionWithTitle:collectionName];
        }
        
        // 2.4 根据传入的相片, 创建相片变动请求
        PHAssetChangeRequest *assetRequest;
        if ([image_Obj isKindOfClass:[UIImage class]]) {
            //如果是图片类型
            assetRequest = [PHAssetChangeRequest creationRequestForAssetFromImage:(UIImage *)image_Obj];
        }
        else if ([image_Obj isKindOfClass:[NSURL class]])
        {
            //如果是URL类型
            assetRequest = [PHAssetChangeRequest creationRequestForAssetFromImageAtFileURL:(NSURL *)image_Obj];
        }
        else
        {
            completion(NO,nil,nil);
            return;
        }
        
        // 2.4 创建一个占位对象
        placeholder = [assetRequest placeholderForCreatedAsset];
        
        // 2.5 将占位对象添加到相册请求中
        [collectionRequest insertAssets:@[placeholder] atIndexes:[NSIndexSet indexSetWithIndex:0]];
        
    } completionHandler:^(BOOL success, NSError * _Nullable error) {
        
        // 3. 回调
        if (completion != nil) {
            completion(success,error,placeholder.localIdentifier);
        }
    }];
}

#pragma mark - 获取相册名称
-(PHAssetCollection *)getCurrentPhotoCollectionWithTitle:(NSString *)collectionName {
    
    // 1. 创建搜索集合
    PHFetchResult *result = [PHAssetCollection fetchAssetCollectionsWithType:PHAssetCollectionTypeAlbum subtype:PHAssetCollectionSubtypeAlbumRegular options:nil];
    
    // 2. 遍历搜索集合并取出对应的相册
    for (PHAssetCollection *assetCollection in result) {
        
        if ([assetCollection.localizedTitle containsString:collectionName]) {
            return assetCollection;
        }
    }
    
    return nil;
}
#pragma mark - 获取图片Data
-(void)getImageData:(PHAsset *)phAsset completion:(nullable void(^)(NSData * imageData))completion
{
    PHImageRequestOptions * imageRequest = [[PHImageRequestOptions alloc]init];
    imageRequest.version = PHImageRequestOptionsVersionUnadjusted;
    
    [[PHImageManager defaultManager] requestImageDataForAsset:phAsset options:imageRequest resultHandler:^(NSData * _Nullable imageData, NSString * _Nullable dataUTI, UIImageOrientation orientation, NSDictionary * _Nullable info) {
        if (completion != nil) {
            completion(imageData);
        }
    }];
}
*/
#if defined (__cplusplus)
extern "C" {
#endif
void HandelCamera(void) {
    [[CameraBridge shared] openCamera];
}
#if defined (__cplusplus)
}
#endif
@end
