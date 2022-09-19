#!/bin/bash
# Execute script via
# bash -x batch.ppfreesurfer.sh

export FREESURFER_HOME="/usr/local/freesurfer-6"
source $FREESURFER_HOME/SetUpFreeSurfer.sh
export LD_PRELOAD=/usr/local/gcc-7.2.0/lib64/libstdc++.so.6

#https://github.com/PennBBL/conte/blob/master/freesurfer/QA.sh
#Runs the quality assurance pipeline for FreeSurfer 5.3 on CFN and outputs several csv's with subject specific QA data. The code is broken down into seven main sections:
# Create subcortical volume segmentation csv - aseg stats
# Create mean QA data charts (thickness and surface area charts)
# Create parcellation csv's - aparc stats
# Create CNR and Euler Numbers csv's
# Flag subjects based on whether they are an outlier (>2 SD) on at least one of the following measures:
# Mean thickness
# Total surface area
# Cortical volume
# Subcortical gray matter
# Cortical White matter
# CNR
# Euler number
# SNR
# ROI- Raw cortical thickness
# ROI- laterality thickness difference
# Flag  (gray/csf flag, gray/white flag, euler number flag, number outliers rois thickness flag, total outliers)
# Create SNR csv
########################################
# This script calls:
# /data/joy/BBL/projects/conteReproc2017/freesurfer/aparc.stats.meanthickness.totalarea.sh
# /data/joy/BBL/projects/conteReproc2017/freesurfer/cnr_euler_number_calculation.sh
# /data/joy/BBL/projects/conteReproc2017/freesurfer/flag_outliers.R
# /data/joy/BBL/projects/conteReproc2017/freesurfer/cnr_euler_qa.R
########################################


## modified by Hyun Joo Yoo, hyunjooy@usc.edu, 08/22/2020

basedir=/ifs/loni/groups/matherlab/Users/Hyunjoo/2018_HRVT_MRI_Study/Result/Freesurfer2
export out_dir=/ifs/loni/groups/matherlab/Users/Hyunjoo/2018_HRVT_MRI_Study/Result/Freesurfer_QC/preOnly176
export SCRIPT_DIR=/ifs/loni/groups/matherlab/Users/Hyunjoo/2018_HRVT_MRI_Study/scripts/Freesurfer/FreeSurferProcess

slist=${SCRIPT_DIR}/sublist_176_cross.txt
echo $slist
for session in pre ; do
output_dir=${out_dir}/${session}
if [ ! -e $output_dir ]; then mkdir -p $output_dir; fi
export SUBJECTS_DIR=${basedir}/${session}
cd $basedir/${session}

#if [ ! -e ${basedir}/${subID}/${session}/mri/lh.amygNucVolumes-T1.v21.txt ]; then

##### 1) Create subcortical volume segmentation csv - aseg stats#####
if [ ! -e "$output_dir/aseg.stats" ]; then
mkdir -p $output_dir/aseg.stats
fi
asegstats2table --subjectsfile=$slist -t  --delimiter comma --tablefile $output_dir/aseg.stats/aseg.stats.volume.txt -m volume --skip
echo bblid scanid >$output_dir/aseg.stats/tempSublist.txt

for i in $(cat $slist);do
bblid=$(echo $i | cut -d"/" -f1)
scanid=$(echo $i | cut -d"/" -f2)
echo $bblid
echo $scanid
echo  ${bblid} ${scanid}  >> $output_dir/aseg.stats/tempSublist.txt
done
#append each subject's sub list into the aseg.txt file
paste $output_dir/aseg.stats/tempSublist.txt $output_dir/aseg.stats/aseg.stats.volume.txt > $output_dir/aseg.stats/aseg.stats.volume.csv

##### 2) Create mean QA data charts (thickness and surface area charts)#####
$SCRIPT_DIR/aparc.stats.meanthickness.totalarea.sh $slist $output_dir $SUBJECTS_DIR

##### 3) Create parcellation csv's - aparc stats#####
aparcstats2table --hemi lh --subjectsfile=$slist -t $output_dir/aparc.stats/lh.aparc.stats.thickness.txt -m thickness --skip
aparcstats2table --hemi rh --subjectsfile=$slist -t $output_dir/aparc.stats/rh.aparc.stats.thickness.txt -m thickness --skip
aparcstats2table --hemi lh --subjectsfile=$slist -t $output_dir/aparc.stats/lh.aparc.stats.volume.txt -m volume --skip
aparcstats2table --hemi rh --subjectsfile=$slist -t $output_dir/aparc.stats/rh.aparc.stats.volume.txt -m volume --skip
aparcstats2table --hemi lh --subjectsfile=$slist -t $output_dir/aparc.stats/lh.aparc.stats.area.txt --skip
aparcstats2table --hemi rh --subjectsfile=$slist -t $output_dir/aparc.stats/rh.aparc.stats.area.txt --skip

paste $output_dir/aseg.stats/tempSublist.txt $output_dir/aparc.stats/lh.aparc.stats.thickness.txt > $output_dir/aparc.stats/lh.aparc.stats.thickness.csv
paste $output_dir/aseg.stats/tempSublist.txt $output_dir/aparc.stats/rh.aparc.stats.thickness.txt > $output_dir/aparc.stats/rh.aparc.stats.thickness.csv
paste $output_dir/aseg.stats/tempSublist.txt $output_dir/aparc.stats/lh.aparc.stats.volume.txt > $output_dir/aparc.stats/lh.aparc.stats.volumes.csv
paste $output_dir/aseg.stats/tempSublist.txt $output_dir/aparc.stats/rh.aparc.stats.volume.txt > $output_dir/aparc.stats/rh.aparc.stats.volumes.csv
paste $output_dir/aseg.stats/tempSublist.txt $output_dir/aparc.stats/lh.aparc.stats.area.txt > $output_dir/aparc.stats/lh.aparc.stats.area.csv
paste $output_dir/aseg.stats/tempSublist.txt $output_dir/aparc.stats/lh.aparc.stats.area.txt > $output_dir/aparc.stats/lh.aparc.stats.area.csv

##### 4) Create CNR and Euler Numbers csv's#####
echo "CNR and Euler Number-already done separately"
$SCRIPT_DIR/Q00_cnr_euler_number_calculation.sh $slist ${SUBJECTS_DIR} ${output_dir}

##### 5) Flag subjects based on whether they are an outlier (>2 SD) on several measures#####


echo "5. Flag subjects based on whether they are an outlier (>2 SD) "
#R --slave --file=$SCRIPT_DIR/flag_outliers.R --args $output_dir

##### 6) Flag  (gray/csf flag, gray/white flag, euler number flag, number outliers rois thickness flag, total outliers)#####
/usr/local/R/bin/R --slave --file=$SCRIPT_DIR/Q00_cnr_euler_qa.R --args $output_dir

#Run script to input the flagged subject csv's and determine is the subject is flagged for automatic FS QA. This script also will output the total number of flags per outlier into a csv table.

auto_flag=$output_dir/all.flags.n1030.csv
euler_flag=$output_dir/cnr_euler_flags_n100.csv
#R --slave --file=$SCRIPT_DIR/sum_flags_auto_qa.R --args $output_dir $auto_flag $euler_flag


###### 7) Create SNR csv#####
##run QA tools recon checker on each subject
#
#export SUBJECTS_DIR="/Users/hyunjooyoo/Documents/Study2017_Office/2018HRVT_MRI/freesurfer/Freesurfer_qdec_long"
#export QA_TOOLS=/Applications/QAtools_v1.2
#echo "QA tools"
#slist=${SCRIPT_DIR}/sublistOA51.txt
#for i in $(cat $slist);do
#echo $i
#/Applications/QAtools_v1.2/recon_checker -s $i -nocheck-aseg -nocheck-status -nocheck-outputFOF -no-snaps
#done > $output_dir/temp.txt
#echo $i "QA running"
##grep for the subject IDs and output into temp2.txt
#grep "wm-anat-snr results" $output_dir/temp.txt | cut -d"(" -f2 | cut -d")" -f1 >$output_dir/temp2.txt
#
##loop through each bblid in temp.txt, and pull the snr value from temp.txt and output to temp3.txt
#for i in $(cat -n $output_dir/temp.txt | grep "wm-anat-snr results" | cut -f1); do
#echo $(sed -n "$(echo $i +2 | bc)p" $output_dir/temp.txt | cut -f1)
#done > $output_dir/temp3.txt
#
##append each subject's snr data into the snr.txt file
#paste temp2.txt temp3.txt > $output_dir/cnr/snr.txt
#
##remove temporary files
#rm -f temp*.txt
#


#fi

done
