# TensorFlowXmlAnnotator
Simple project to update tensorflow image annotations after the resizing the image


Created this project so that the repetetive image annotation task could be automated.
Since it is the initial checkin problems faced during the tensorflow image annotations are addressed

Steps to Follow,

Most of the work is configured via the App.config as seen below,

<appSettings>
    <add key="renameFiles" value="true"/>
    <add key="resizingValue" value="20"/>
    <add key="allowedSizeInKb" value="7"/>
    <add key="backupContent" value="true"/>
    <add key="annotationLocation" value="D:\xyz\sample_data\foods\train\Annotations"/>
    <add key="imageLocation" value="D:\xyz\sample_data\sample_data\foods\train\Images"/>
    <add key="annotationsBackup" value="D:\xyz\sample_data\sample_data\foods\train\AnnotationsBackup"/>
    <add key="useImageDimension" value="false"/>
    <add key="useFileSize" value="true"/> wip
    <add key="widthThreshold" value=""/> wip
    <add key="heightThreshold" value=""/> wip
  </appSettings>
  
Settings Explained,
1. renameFiles - When enabled, app will rename the filenames i.e. will replace spaces( ) with underscores(_)
2. resizingValue - Value in percentage, Ideally this should be the same value as your image resizing percentage.
   so that the coordinates result in minimum error.
3. allowesSizeInKb - This represents the size of the xml file in kb, will update it further to use the image size as a reference.
4. backupContent - When enabled, the original files will be backed up in the location specified in another setting (i.e. annotationsBackup)
5. annotationLocation - The folder location where the annotations(.xml) files are stored.
6. imageLocation - the folder location where the training images are stored
7. annotationsBackup - see 4
8. useImageDimension - when enabled, we will use the image dimension instead of xml size for updating the annotations.(WIP)
9. useFileSize - when enabled, file size will be used for updating the annotations (WIP), allowedSizeInKb will be used to refer the size.
10,11. widthThreshold/heightThreshold - these will be useful when setting-8 is enabled, and will be used to filter annotations based on the 
threshold values (WIP)

:)
