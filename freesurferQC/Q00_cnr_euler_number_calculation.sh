#! /bin/bash
export FREESURFER_HOME="/usr/local/freesurfer-6"
source $FREESURFER_HOME/SetUpFreeSurfer.sh
export LD_PRELOAD=/usr/local/gcc-7.2.0/lib64/libstdc++.so.6

#This script will be used to create aggregate files with CNR and Euler number values for freesurfer version 5.3
#The methods for this script were obtained from Chalavi, et al, BMC medical Imaging, 2012. doi:10.1186/1471-2342-12-27.(http://www.biomedcentral.com/1471-2342/12/27).
#Previous BBL scripts to calculate cnr only pulled the total cnr, in this script we pull total, gray/csf for left and right hemispheres and gray/white for both hemispheres in addition to calculating the euler number (tells how many holes/defects are in the surface) as is done in the methods of the Chalavi, et al paper



## modified by Hyun Joo Yoo, hyunjooy@usc.edu, 08/22/2020


#set freesurfer specific variables
basedir=/ifs/loni/groups/matherlab/Users/Hyunjoo/2018_HRVT_MRI_Study/Result/Freesurfer2
#export out_dir=/Users/hyunjooyoo/Documents/Study2017_Office/2018HRVT_MRI/freesurfer_QC
#export SUBJECTS_DIR=${basedir}

export SCRIPT_DIR=/ifs/loni/groups/matherlab/Users/Hyunjoo/2018_HRVT_MRI_Study/scripts/Freesurfer/FreeSurferProcess
export SUBJECTS_DIR=$2
export OUTPUT_DIR=$3

#slist=${SCRIPT_DIR}/sublist_151_cross.txt
slist=$1
#set subject list, directory to output aggregated files to, and the filenames of those aggregate files
#slist=/data/joy/BBL/studies/conte/subjectData/design3FullSubjectList.txt
#slist=$1
#outdir=/data/joy/BBL/studies/conte/subjectData/freesurfer/stats
outdir=$OUTPUT_DIR/cnr
if [ ! -e ${outdir} ]; then
	mkdir -p $outdir
fi

file=${outdir}/cnr_buckner.csv
euler_file=${outdir}/euler_number.csv

#create the aggregate files for cnr and euler
echo "bblid,scanid,total,graycsflh,graycsfrh,graywhitelh,graywhiterh" > $file
echo "bblid,scanid,left_euler,right_euler,left_holes,right_holes,left_total_defect_index,right_total_defect_index" > $euler_file

#for each subject in the subject list, get bblid, scanid (note it is actually datexscanid) and their surf and mri directories
for i in $(cat $slist);do
	bblid=$(echo $i | cut -d"/" -f1)
	scanid=$(echo $i | cut -d"/" -f2)
echo $bblid
echo $scanid
	echo "working on subject" $i
	surf=`ls -d $SUBJECTS_DIR/$i/surf`
	mri=`ls -d $SUBJECTS_DIR/$i/mri`
echo $surf
echo $mri
		###############CNR#######################
		#calculate cnr for each subject and output all measures to a file in their stats folder
echo $i "cnr start"
#SUBJECTS_DIR=/Users/hyunjooyoo/Documents/Study2017_Office/2018HRVT_MRI/freesurfer/Freesurfer_qdec_long
mri_cnr $surf $mri/orig.mgz &> ${SUBJECTS_DIR}/${i}/stats/${bblid}"_"${scanid}"_cnr.txt"
		#create variables for total cnr, gray/csf for left and right hemispheres and gray/white for left and right hemispheres (total and cnr variables are grepping the information from the subject specific file, the variables then need to be cut in order to get just the number)
		total=`grep "total CNR" $SUBJECTS_DIR/$i/stats/$bblid"_"$scanid"_cnr.txt"`
		total2=`echo $total |cut -f 4 -d " "`
		cnr=`grep "gray/white CNR" $SUBJECTS_DIR/$i/stats/$bblid"_"$scanid"_cnr.txt"`
		graycsflh=`echo $cnr | cut -d "," -f 2 | cut -d "=" -f 2 | cut -d " " -f 2`
		graycsfrh=`echo $cnr | cut -d "," -f 3 | cut -d "=" -f 2 | cut -d " " -f 2`
		graywhitelh=`echo $cnr | cut -d "," -f 1 | cut -d "=" -f 2 | cut -d " " -f 2`
		graywhiterh=`echo $cnr | cut -d "," -f 2 | cut -d "=" -f 3 | cut -d " " -f 2`
		#append this subject's data to the aggregate file
		echo $bblid,$scanid,$total2,$graycsflh,$graycsfrh,$graywhitelh,$graywhiterh >> $file

		###############EULER NUMBER#######################
		#calculate the euler number and output left and right hemisphere euler numbers to a file in their stats folder
	#	echo $SUBJECTS_DIR/$i/stats/$bblid"_"$scanid"_lh_euler.txt"
	#	echo $SUBJECTS_DIR/$i/stats/$bblid"_"$scanid"_rh_euler.txt"

echo $i "Euler number start"
		mris_euler_number $surf/lh.orig.nofix &> $SUBJECTS_DIR/$i/stats/$bblid"_"$scanid"_lh_euler.txt"
		mris_euler_number $surf/rh.orig.nofix &> $SUBJECTS_DIR/$i/stats/$bblid"_"$scanid"_rh_euler.txt"
		#create variables for left and right euler numbers
		left=`grep ">" $SUBJECTS_DIR/$i/stats/$bblid"_"$scanid"_lh_euler.txt" | cut -d ">" -f 1 | cut -d "=" -f 4 | cut -d " " -f 2`
		right=`grep ">" $SUBJECTS_DIR/$i/stats/$bblid"_"$scanid"_rh_euler.txt" | cut -d ">" -f 1 | cut -d "=" -f 4 | cut -d " " -f 2`
		leftH=`grep "holes" $SUBJECTS_DIR/$i/stats/$bblid"_"$scanid"_lh_euler.txt" | cut -d ">" -f 2 | cut -d "h" -f 1`
		rightH=`grep "holes" $SUBJECTS_DIR/$i/stats/$bblid"_"$scanid"_rh_euler.txt" | cut -d ">" -f 2 | cut -d "h" -f 1`
	leftDefect=`grep "defect" $SUBJECTS_DIR/$i/stats/$bblid"_"$scanid"_lh_euler.txt" | cut -d "=" -f 2`
	rightDefect=`grep "defect" $SUBJECTS_DIR/$i/stats/$bblid"_"$scanid"_rh_euler.txt" | cut -d "=" -f 2`
		#append this subject's data to the aggregate file
		echo  ${bblid},${scanid},${left},${right},${leftH},${rightH},${leftDefect},${rightDefect}>> $euler_file

done #for i in $(cat $slist);do
