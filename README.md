# Important

This is a beta release. Bugs are to be expected. If you notice any issues, please submit bug reports to help us improve
the software.

![](https://raw.githubusercontent.com/FEntwumS/Oneware.NetlistReaderFrontend/refs/heads/master/assets/netlist-view.jpg)

# About

This repository contains two plugins for [OneWare Studio](https://github.com/one-ware/OneWare). They are developed by
TH Köln (Cologne University of Applied Sciences) as part of
the [FEntwumS research project](https://www.th-koeln.de/informations-medien-und-elektrotechnik/forschungsprojekt-fentwums_121126.php).

## Netlist viewer

The FEntwumS Netlist Viewer plugin allows users to automatically generate and interactively view netlists for their HDL
designs inside OneWare Studio. It is a frontend building on
the [FEntwumS Netlist Reader Backend](https://github.com/FEntwumS/NetlistReaderBackend). You can run the backend, which
does all the layouting, either on your local machine or on a remote server in your local network. The default (and
recommended) comfiguration is to run the backend locally. If this is all you need, you just need to install the plugin.
It will install and start the backend automatically.

### Using a remote backend

If you plan on using a local backend install (the default and recommended case), you can skip this section.

If you want to use a remote backend, you will need to deploy it yourself. Instructions on how to do so can be
found [here](https://github.com/FEntwumS/NetlistReaderBackend?tab=readme-ov-file#can-i-use-the-backend-separately-from-the-frontend).
Since traffic between the backend and the viewer is neither encrypted nor authenticated, you should only use a remote
backend within local and trusted networks.

When you have your backend up and running, you will need to enter the backends IP address and port
(the default port is 8080) in the settings. You also need to check the box for "Use remote backend".

### Viewing your design

Now that you have installed the plugin, you can start viewing and interacting with your HDL designs. By simply
right-clicking your top level entity and choosing the "View netlist for [insert your entity here]", the plugin will
automatically generate a netlist using yosys and then display it in a new tab. You can zoom using your mouse wheel,
clicking and dragging with the left mouse button pans the view and left-clicking on an entity will either expand or
collapse it (depending on whether the entity is collapsed or not).

### Viewing a generated netlist

There are certain requirements that a netlist needs to fulfill to be viewable by the netlist viewer. At a minimum, it
needs to be flattened (preferably using the `-scopename` option), the toplevel entity needs to be specified using the
`hierarchy` command and all blackbox cells such as device specific primitives need to have port direction information.
The netlist file needs to be of the `.json` filetype. To ensure that device specific primitives contain information on
port directions, load the appropriate libraries. For example, to add this information for the device primitives of the
Cologne Chip Gatemate series of FPGAs you need to run
`read_verilog -lib -specify +/gatemate/cells_sim.v +/gatemate/cells_bb.v`. The appropriate command can be found on the
[synthesis page](https://yosyshq.readthedocs.io/projects/yosys/en/latest/cmd/synth_gatemate.html) for the FPGA you are
using.

The recommended command to generate a netlist is
`hierarchy -check -top <top>; proc; memory -nomap; flatten -scopename; write_json -compat-int <top>.json`. This sequence
expects the HDL files to be fully loaded.

# Troubleshooting

## ERROR: Module ... is not part of the design

Please make sure that you have specified the correct device manufacturer and device family in the settings.
The available options are listed on the synth_<...> pages in
the [Yosys documentation](https://yosyshq.readthedocs.io/projects/yosys/en/latest/cmd_ref.html)

## I generated a netlist using the OneWare Studio command, but I can't see all my entity instances

The builtin Json Netlist generation does not output a flattened netlist. If you want to expand your entities, you need
to right-click your toplevel entity and use the "View netlist for [...]" option.

## I generated a netlist myself, but I can't seem to find it in the project explorer

By default, OneWare Studio does not display `.json` files in the project explorer.
