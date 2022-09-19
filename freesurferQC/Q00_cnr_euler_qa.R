
#This script takes the CNR and Euler number output from 
#cnr_euler_number_calculation.sh
#for Freesurfer processing and calculates binary flags for the euler number and cnr subcategories

############################################
###########DATA PREP########################

#read in data
#output.dir<-"/data/joy/BBL/studies/conte/subjectData/freesurfer/stats"
output.dir<-commandArgs(TRUE)[1]
cnr_data<- read.csv(paste(output.dir,"/cnr/cnr_buckner.csv",sep=""))
euler_data<- read.csv(paste(output.dir,"/cnr/euler_number.csv",sep=""))

#merge the files together by datexscanid
data<- cnr_data
data$left_euler<- euler_data$left_euler[match(data$scanid,euler_data$scanid)]
data$right_euler<- euler_data$right_euler[match(data$scanid,euler_data$scanid)]

#################################################
###########BINARY EXCLUSION FLAGS################

#create a dataframe which will get the flags based on euler and cnr calculations
flags<- data

#get mean values for gray/csf cnr, gray/white cnr and euler numbers (average across hemispheres)
flags$mean_euler<-(flags$left_euler+flags$right_euler)/2
flags$mean_graycsf_cnr<- (flags$graycsflh+flags$graycsfrh)/2
flags$mean_graywhite_cnr<- (flags$graywhitelh+flags$graywhiterh)/2

#subset data frame to only IDs and averages
flags<- flags[,c(1,2,10:12)]

#create variables that get the standard deviation for cnr and euler number averages
graycsf_cutoff<- mean(flags$mean_graycsf_cnr-(2*sd(flags$mean_graycsf_cnr,na.rm=T)),na.rm=T)
graywhite_cutoff<- mean(flags$mean_graywhite_cnr-(2*sd(flags$mean_graywhite_cnr,na.rm=T)),na.rm=T)
euler_cutoff<- mean(flags$mean_euler-(2*sd(flags$mean_euler,na.rm=T)),na.rm=T)

#create a binary flag column (1=yes, 0=no) for average cnr and euler numbers (<2 SD =1, >2 SD=0)
flags$graycsf_flag<- NA
flags$graywhite_flag<- NA
flags$euler_flag<- NA

#remove any subjects with no data
flags<- flags[! is.na(flags$mean_euler),]

for (i in 1:nrow(flags)){
  if (flags$mean_graycsf_cnr[i]<= graycsf_cutoff){
flags$graycsf_flag[i]<- 1
} else if (flags$mean_graycsf_cnr[i]>graycsf_cutoff){
flags$graycsf_flag[i]<- 0 
}

if (flags$mean_graywhite_cnr[i]<=graywhite_cutoff){
  flags$graywhite_flag[i]<- 1
} else if (flags$mean_graywhite_cnr[i]>graywhite_cutoff){
  flags$graywhite_flag[i]<- 0 
} 

if (flags$mean_euler[i]<=euler_cutoff){
  flags$euler_flag[i]<- 1
} else if (flags$mean_euler[i]>euler_cutoff){
  flags$euler_flag[i]<- 0 
}

} # for (i in 1:nrow(flags)){

#subset data frame to only IDs and flags
flags<- flags[,c(1,2,6:8)]

#create a total outliers column which gets the number of total outliers and a column which gets a binary flag 1=yes, 0=no
flags$total_outliers<- NA
x<- cbind(flags$graycsf_flag,flags$graywhite_flag,flags$euler_flag)
flags$total_outliers<- apply(x,1,sum)
flags$flagged<- flags$total_outliers
flags$flagged[flags$flagged>0]<- 1

nsubj<- nrow(flags)

#write out flagged data
write.csv(flags,paste(output.dir,"/cnr/cnr_euler_flags_n",nsubj,".csv",sep=""))
#write.csv(flags,paste("/Users/hyunjooyoo/Documents/Study2017_Office/2018HRVT_MRI/freesurfer/QA/cnr_euler_flags_n",nsubj,".csv",sep=""))

