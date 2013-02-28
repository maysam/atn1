function [EF] = EF_undirdir(mLinks, F , pn, verbose)
% converting mLinks and F into doubles
mLinks = double(mLinks);
F = double(F);

% creates sparse matrix - mLinks(:,1) is the source (first col of mLinks)
% mLinks(:,2) is the destination (2nd col of mLinks)
% So (i(k), j(k)) = F(k)
Z = sparse(mLinks(:,1),mLinks(:,2),F,pn,pn);  

% Set diag to zero
v=diag(Z); v=-v; v=diag(v); Z=Z+v;
clear v

% for each node, sum in and out citations to get weight of each node
p = zeros(1,pn);
for i=1:pn
   p(i) = sum(Z(:,i)) + sum(Z(i,:)); 
end

% normalize p
p(1,:)=p(1,:)/sum(p(1,:));

if (verbose == 1)
    fprintf('p (normalized)\n');
    disp(p);
end

% make Z matrix ROW stochastic
Z = Z';
s=sum(Z); 
x=find(s ~= 0);
for i=1:numel(x)
   Z(:,x(i))=Z(:,x(i))./s(x(i)); 
end
Z = Z';

% take one step on network using weights
EF=(p*Z)';

if (verbose == 1)
    fprintf('Row stochastic Z\n');
    disp(Z);
    
    fprintf('EF before normalization\n');
    disp(EF);
end

% normalize the data so the sum of all EF scores equals 1
EF(:,1)=EF(:,1)/sum(EF(:,1));







