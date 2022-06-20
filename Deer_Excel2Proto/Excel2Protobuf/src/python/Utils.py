import os
import shutil


def log(log_data):
    print("\033[m" + log_data)


def logError(error_data):
    print("\033[0;32;31m" + error_data)


def makedir(dir2make):
    if not os.path.exists(dir2make):
        os.makedirs(dir2make)


def rmdir(dir2remove):
    if os.path.exists(dir2remove):
        shutil.rmtree(dir2remove, True)


def copyfile(src, des):
    shutil.copy(src, des)

# 遍历文件夹及其子文件夹中的文件，并存储在一个列表中
# 输入文件夹路径、空文件列表[]
# 返回 文件列表Filelist,包含文件名（完整路径）
def get_filelist(dir, filelist):
    newDir = dir
    if os.path.isfile(dir):
        filelist.append(dir)
        # # 若只是要返回文件文，使用这个
        # Filelist.append(os.path.basename(dir))
    elif os.path.isdir(dir):
        for s in os.listdir(dir):
            # 如果需要忽略某些文件夹，使用以下代码
            #if s == "xxx":
            #continue
            newDir=os.path.join(dir,s)
            get_filelist(newDir, filelist)
