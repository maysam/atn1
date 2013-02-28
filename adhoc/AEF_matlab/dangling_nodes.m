function [vT,D] = dangling_nodes(mLinks,F,pn,ln,A)

C1 = cell(pn,1);
for i=1:ln
    h = mLinks(i,1);
    C1{h} = [C1{h},i];  
end

%dD: dangling node vector
D = zeros(pn,1);
for i=1:pn
   if (isempty(C1{i}));
      D(i) = 1;
   end
end

% Fast Container, C2: N --> mLinks(:,2) indices
C2 = cell(pn,1);
for i=1:ln
    h = mLinks(i,2);
    C2{h} = [C2{h},i];
end

% teleport by links
vT = zeros(1,pn);
for i=1:pn
    x = C2{i};
    vT(i) = sum(F(x));
end
vT = vT./sum(vT);

end






