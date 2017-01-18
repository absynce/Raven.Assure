const _ = require('lodash/fp');
const args = require('yargs').argv;
const fs = require('fs');
const gulp = require('gulp');
const gutil = require('gulp-util');
const path = require('path');
const proc = require('child_process');
const usage = require('gulp-help-doc');

var paths = {
  bin: {
    prefix: 'src/Raven.Assure/bin',
    environment: 'Release',
    target: 'net46',
    files: '**/*'
  },
  package: 'package'
};

gulp.task('help', () => usage(gulp));

/**
 * Build/compile Raven.Assure.
 * Same as `dotnet build ./src/Raven.Assure/`.
 *
 * @task {build}
 */
gulp.task('build', function buildRavenAssure(done) {
   var buildProcess = proc
      .spawn('dotnet', ['build'], {
         cwd: path.join('src', 'Raven.Assure')
      });

   buildProcess.stdout.on('data', logDatar);
   buildProcess.stderr.on('data', logError);
   buildProcess.on('close', done);
});

/**
 * Run Raven.Assure.Test xUnit tests.
 * Same as `dotnet test ./test/Raven.Assure.Test/`.
 *
 * @task {test}
 */
gulp.task('test', function testRavenAssure(done) {
   var testProcess = proc
      .spawn('dotnet', ['test'], {
         cwd: path.join('test', 'Raven.Assure.Test')
      });


   testProcess.stdout.on('data', logDatar);
   testProcess.stderr.on('data', logError);
   testProcess.on('close', done);
});

/**
 * Package Raven.Assure CLI into ./package/{target}/smuggle-{version}-{environment}.
 * @task {package}
 * @arg {environment} The build environment: {Debug|Release(default)}
 *
 * TODO: add branch to package output folder name, like with mobile.
 */
gulp.task('package', function packageRavenAssure(done) {
  const smugglePackage = getPackageJson();
  var env = args.environment || paths.bin.environment;
  var fullPackagePath = path.join(paths.package, paths.bin.target, `smuggle-${smugglePackage.version}-${env}`);
  var fullBinPath = path.join(paths.bin.prefix, env, paths.bin.target, paths.bin.files);

  gutil.log('Packaging from:', gutil.colors.cyan(fullBinPath));
  gutil.log('Packaging into:', gutil.colors.cyan(fullPackagePath));

  gulp.src(fullBinPath)
      .pipe(gulp.dest(fullPackagePath));
});

// Define reusable loggers.
var toString = (msg) => msg.toString();
// dotnet CLI adds line break, so when combined with log it adds an additional empty line.
// This replaces that line break with an empty string.
var removeLineBreaks = (msg) => msg.replace(/(\r\n|\r|\n)/g, '');
var logDatar = _.flow(toString, removeLineBreaks, gutil.log);
var logError = _.flow(gutil.colors.red, logDatar);

var getPackageJson = () => JSON.parse(fs.readFileSync('./package.json', 'utf8'));
