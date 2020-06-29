png(filename = filename)
samplesize=length(检测系统A结果)

x<-检测系统A结果
y<-检测系统B结果

# 离群值
z<-abs(y-x)
z1<-4*mean(z)
out=0 #个数
ID<-c() # 下标 从1开始
for (q in 1:samplesize){
  if (z[q]>z1){
    out=out+1
    ID<-append(ID,q)
  }
}

# round2 = function(x, n) {
#   posneg = sign(x)
#   z = abs(x)*10^n
#   z = z + 0.5
#   z = trunc(z)
#   z = z/10^n
#   z*posneg
# }

lx<-samplesize
l<-choose(lx,2) #所有点中任取两点的排列组合总数

S<-matrix(1:l,nrow=1,ncol=l)
for (i in 1:(lx-1)) {
  for (j in (i+1):lx) {
    index<-(j-i)+(i-1)*(lx-i/2)
    S[index]<-(y[i]-y[j])/(x[i]-x[j])
  }
}


S.sort<-sort(S)
S.sort1<-subset(S.sort,S.sort!=0)
# S.sort1<-S.sort
S.sort2<-subset(S.sort,S.sort+1<0)
S.sort3<-subset(S.sort,S.sort==-1)
S.sort4<-subset(S.sort,S.sort==0)
S.sort5<-subset(S.sort,S.sort==1)
S.sort6<-subset(S.sort,S.sort>1)

# N<-length(S.sort1)
N<-length(S.sort)+length(S.sort3)
neg<-length(subset(S.sort,S.sort<0))
K<-length(S.sort2)
if (N%%2==0) { 
  N1<-N/2 
  b<-(S.sort1[N1+K-length(S.sort3)+1]+S.sort1[N1+K+1-length(S.sort3)+1])/2
} else if (N%%2==1) { 
  N1<-(N-1)/2 
  b<-S.sort1[N1+K+1-length(S.sort3)+1]
} else { 
  N1<-"Neither!" 
} 

a<-median(y-b*x)
#CI of b
C.gamma<-qnorm(0.975)*sqrt(lx*(lx-1)*(2*lx+5)/18)
M1<-round((N-C.gamma)/2)
M2<-N-M1+1
b.lower<-S.sort1[M1+K-length(S.sort3)+1]
b.upper<-S.sort1[M2+K-length(S.sort3)+1]
#CI of a
a.lower<-median(y-b.upper*x)
a.upper<-median(y-b.lower*x)
result<-list(intercept=a,intercept.CI=c(a.lower,a.upper),slope=b,slope.CI=c(b.lower,b.upper))


# re<-PB.reg(data)
# 
# 
# b<-re$slope
# a<-re$intercept
nneg=0
npos=0
for (i in 1:samplesize){
  if(b*x[i]+a>y[i]){
    nneg<-nneg+1
  } else if(b*x[i]+a<y[i]){
    npos<-npos+1
  }
}

r<-c()
for (j in 1:samplesize){
  if(b*x[j]+a>y[j]){
    r[j]=-sqrt(npos/nneg)
  } else if(b*x[j]+a<y[j]){
    r[j]=sqrt(nneg/npos)
  } else(
    r[j]=0
  )
}

D<-c()
for (k in 1:samplesize){
  k1<-y[k]+x[k]/b-a
  k2<-sqrt(1+1/b^2)
  D<-append(D,k1/k2)
}

E<-order(D)
r1<-r[E]

cu<-c()
for (s in 1:samplesize){
  cu<-append(cu,sum(r1[1:s]))
}

h<-max(abs(cu))/sqrt(nneg+1)
# 
# SE1=re$intercept+(re$slope-1)*xc1
# SEmin1=re$intercept.CI[1]+(re$slope.CI[1]-1)*xc1
# SEmax1=re$intercept.CI[2]+(re$slope.CI[2]-1)*xc1
# 
# SE2=re$intercept+(re$slope-1)*xc2
# SEmin2=re$intercept.CI[1]+(re$slope.CI[1]-1)*xc2
# SEmax2=re$intercept.CI[2]+(re$slope.CI[2]-1)*xc2

if (h<1.358099){
  p=0.11
} else {
  p=0.09
} 

# 作图
plot(x,y,pch=21,bg="gray",main="",
     xlab = target,ylab = match)
abline(a,b,col="blue")
abline(a.lower,b.lower,col="red",lty=2)
abline(a.upper,b.upper,col="red",lty=2)
dev.off()

# correl<-paste("R = ",round(cor.test(x,y)$estimate,3),sep="")
# slope<-paste("Slope = ",round(b,2)," [",round(b.lower,2),",",round(b.upper,2),"]",sep="")
# intercept<-paste("Intercept = ",round(a,2)," [",round(a.lower,2),",",round(a.upper,2),"]",sep="")
# text<-c(correl,slope,intercept)
# legend("topleft",text,title="Regression Data",inset = .05)
# source("仪器比对报告.R")