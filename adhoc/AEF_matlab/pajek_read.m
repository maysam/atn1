function [N,L] = pajek_read(fn)

% get number of lines in pajek file
cmdstr = ['wc -l < ',fn];
[s,w] = unix(cmdstr);
nL = str2num(w);

% find '*Arcs' 
cmdstr = ['sed -n ''/^*Arcs/='' ',fn];
[s,w] = unix(cmdstr);
u = str2num(w) - 2;

%% Vertices
fid = fopen(fn);
N = textscan(fid,'%u %q',u+1,'HeaderLines',1); 
fclose(fid);

%% Edges
fid = fopen(fn);
L = textscan(fid,'%u %u %u','HeaderLines',u+2);
fclose(fid);



